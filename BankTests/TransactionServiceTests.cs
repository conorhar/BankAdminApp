using System;
using AutoFixture;
using BankAdminApp.Services.Accounts;
using BankAdminApp.Services.Transactions;
using BankAdminApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharedThings;
using SharedThings.Models;
using SharedThings.Services.Customers;

namespace BankTests
{
    [TestClass]
    public class TransactionServiceTests
    {
        private TransactionService sut;
        private ApplicationDbContext context;
        private Mock<ICustomerService> customerServiceMock;
        private Mock<IAccountService> accountServiceMock;

        public TransactionServiceTests()
        {
            context = CreateDbContext();
            customerServiceMock = new Mock<ICustomerService>();
            accountServiceMock = new Mock<IAccountService>();
            sut = new TransactionService(context, customerServiceMock.Object, accountServiceMock.Object);
        }

        ApplicationDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            var dbContext = new ApplicationDbContext(options);

            var fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            dbContext.AddRange(fixture.CreateMany<Account>(20));
            dbContext.SaveChanges();

            return dbContext;
        }
        
        [TestMethod]
        public void Transaction_amount_should_never_be_negative()
        {
            var model = new TransactionConfirmViewModel
            {
                AccountId = 1,
                Operation = "Operation",
                Amount = 100,
                Type = "Type"
            };

            var transaction = sut.CreateTransaction(model);

            Assert.AreEqual(100, transaction.Amount);
        }
    }
}
