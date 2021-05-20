using System;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharedThings.Data;
using SharedThings.Services.Accounts;
using SharedThings.Services.Customers;
using SharedThings.Services.Transactions;
using SharedThings.ViewModels;

namespace BankAdminApp.Tests.Services
{
    [TestClass]
    public class TransactionServiceTests : BaseTest
    {
        private ITransactionService sut;

        private Mock<ICustomerService> customerServiceMock;
        private Mock<IAccountService> accountServiceMock;
        private ApplicationDbContext ctx;

        public TransactionServiceTests()
        {
            customerServiceMock = new Mock<ICustomerService>();
            accountServiceMock = new Mock<IAccountService>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;
            ctx = new ApplicationDbContext(options);

            sut = new TransactionService(ctx, customerServiceMock.Object, accountServiceMock.Object);
        }

        [TestMethod]
        public void WhenTransactionTypeIsDebitBalanceShouldDecrease()
        {
            var viewModel = fixture.Create<TransactionConfirmViewModel>();
            viewModel.Type = "Debit";

            accountServiceMock.Setup(e => e.GetBalance(It.IsAny<int>())).Returns(100m);

            var result = sut.CreateTransaction(viewModel);

            Assert.IsTrue(result.Balance < 100);
        }

        [TestMethod]
        public void WhenTransactionTypeIsCreditBalanceShouldIncrease()
        {
            var viewModel = fixture.Create<TransactionConfirmViewModel>();
            viewModel.Type = "Credit";

            accountServiceMock.Setup(e => e.GetBalance(It.IsAny<int>())).Returns(100m);

            var result = sut.CreateTransaction(viewModel);

            Assert.IsTrue(result.Balance > 100);
        }
    }
}