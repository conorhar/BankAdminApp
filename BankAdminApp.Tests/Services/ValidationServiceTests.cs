using System;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharedThings.Data;
using SharedThings.Models;
using SharedThings.Services.Validation;
using SharedThings.ViewModels;

namespace BankAdminApp.Tests.Services
{
    [TestClass]
    public class ValidationServiceTests : BaseTest
    {
        private IValidationService sut;
        private ApplicationDbContext ctx;

        public ValidationServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;
            ctx = new ApplicationDbContext(options);
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            sut = new ValidationService(ctx);
        }

        [TestMethod]
        public void BalanceIsInsufficientValidationShouldReturnTrueWhenTypeIsDebitAndBalanceIsInsufficient()
        {
            var account = fixture.Create<Account>();
            account.Balance = 50;
            ctx.Add(account);
            ctx.SaveChanges();

            var result = sut.BalanceIsInsufficient(100, account.AccountId, "Debit");

            Assert.IsTrue(result);
        }
    }
}