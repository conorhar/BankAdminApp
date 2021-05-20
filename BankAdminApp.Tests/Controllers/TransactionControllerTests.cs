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

            sut = new TransactionController(ctx, transactionServiceMock.Object, customerServiceMock.Object, accountServiceMock.Object,
                validationServiceMock.Object);
        }

        [TestMethod]
        public void ChooseAmountValidationEnsuresTransactionAmountCannotBeNegative()
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
