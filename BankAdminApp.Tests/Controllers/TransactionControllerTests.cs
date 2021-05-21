using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoFixture;
using BankAdminApp.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharedThings.Data;
using SharedThings.Models;
using SharedThings.Services.Accounts;
using SharedThings.Services.Customers;
using SharedThings.Services.Transactions;
using SharedThings.Services.Validation;
using SharedThings.ViewModels;

namespace BankAdminApp.Tests
{
    [TestClass]
    public class TransactionControllerTests : BaseTest
    {
        private TransactionController sut;
        private Mock<ITransactionService> transactionServiceMock;
        private Mock<ICustomerService> customerServiceMock;
        private Mock<IAccountService> accountServiceMock;
        private Mock<IValidationService> validationServiceMock;
        private ApplicationDbContext ctx;

        public TransactionControllerTests()
        {
            transactionServiceMock = new Mock<ITransactionService>();
            customerServiceMock = new Mock<ICustomerService>();
            accountServiceMock = new Mock<IAccountService>();
            validationServiceMock = new Mock<IValidationService>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;
            ctx = new ApplicationDbContext(options);

            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            sut = new TransactionController(ctx, transactionServiceMock.Object, customerServiceMock.Object, accountServiceMock.Object,
                validationServiceMock.Object);
        }

        [TestMethod]
        public void WhenConfirmIsCalledCreateTransactionForReceiverIsCalledIfInternalTransfer()
        {
            var viewModel = fixture.Create<TransactionConfirmViewModel>();

            var account = fixture.Build<Account>()
                .With(x => x.AccountId, viewModel.AccountId)
                .Create<Account>();

            var transaction = fixture.Build<Transaction>()
                .With(x => x.Operation, "Remittance to Internal Account")
                .Create<Transaction>();

            transactionServiceMock.Setup(e => e.CreateTransaction(It.IsAny<TransactionConfirmViewModel>()))
                .Returns(transaction);

            transactionServiceMock.Setup(e => e.CreateTransactionForReceiver(It.IsAny<TransactionConfirmViewModel>()))
                .Returns(transaction);

            ctx.Add(account);
            ctx.SaveChanges();

            sut.Confirm(viewModel);

            transactionServiceMock.Verify(e => 
                e.CreateTransactionForReceiver(It.IsAny<TransactionConfirmViewModel>()), Times.Once);
        }

        [TestMethod]
        public void WhenConfirmIsCalledCreateTransactionShouldNotBeCalledIfModelIsNotValid()
        {
            var viewModel = fixture.Create<TransactionConfirmViewModel>();
            sut.ModelState.AddModelError("Amount", "Test message");

            sut.Confirm(viewModel);

            transactionServiceMock.Verify(e => 
                e.CreateTransaction(It.IsAny<TransactionConfirmViewModel>()), Times.Never);
        }

        [TestMethod]
        public void ChooseAmountValidationShouldEnsureTransactionAmountCannotBeNegative()
        {
            var viewModel = fixture.Create<TransactionChooseAmountViewModel>();

            viewModel.Amount = -1m;

            Assert.IsTrue(ValidateModel(viewModel).Any(
                v => v.MemberNames.Contains("Amount") &&
                     v.ErrorMessage.Contains("Minimum transaction is 0,01")));
        }
        
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }
    }
}
