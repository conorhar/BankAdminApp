using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using BankAdminApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharedThings.Data;
using SharedThings.Models;
using SharedThings.Services.Api;
using SharedThings.Services.Customers;
using SharedThings.Services.Search;
using SharedThings.ViewModels;

namespace BankAdminApp.Tests
{
    [TestClass]
    public class CustomerControllerTests : BaseTest
    {
        private CustomerController sut;
        private Mock<ICustomerService> customerServiceMock;
        private Mock<ISearchService> searchServiceMock;
        private Mock<IApiService> apiServiceMock;
        private ApplicationDbContext ctx;

        public CustomerControllerTests()
        {
            customerServiceMock = new Mock<ICustomerService>();
            searchServiceMock = new Mock<ISearchService>();
            apiServiceMock = new Mock<IApiService>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;
            ctx = new ApplicationDbContext(options);

            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            sut = new CustomerController(ctx, customerServiceMock.Object, searchServiceMock.Object,
                apiServiceMock.Object);
        }

        [TestMethod]
        public void CheckCorrectValuesAreAssignedInCustomerDetails()
        {
            var customer = fixture.Build<Customer>().Without(r => r.Dispositions).Create<Customer>();
            var account = fixture.Build<Account>().Without(r => r.Dispositions).Create<Account>();
            var disposition = new Disposition
            {
                Account = account,
                Customer = customer,
                Type = "OWNER"
            };
            customer.Dispositions.Add(disposition);
            ctx.Customers.Add(customer);
            ctx.Accounts.AddRange(account);
            ctx.SaveChanges();

            customerServiceMock.Setup(e => e.GetFullAddress(It.IsAny<Customer>())).Returns("Full address");
            customerServiceMock.Setup(e => e.GetFullName(It.IsAny<Customer>())).Returns("Full name");
            customerServiceMock.Setup(e => e.GetFullTelephoneNumber(It.IsAny<Customer>())).Returns("Telephone number");
            customerServiceMock.Setup(e => e.GetNationalIdOutput(It.IsAny<Customer>())).Returns("National id");
            customerServiceMock.Setup(e => e.GetAccounts(customer.CustomerId)).Returns(GetAccounts(customer));
            customerServiceMock.Setup(e => e.GetAccountOwnershipInfo(It.IsAny<int>(), It.IsAny<int>()))
                .Returns("OWNER");

            var fakeAccounts = GetAccounts(customer);
            var fakeAccountsItems = fakeAccounts.Select(r => new CustomerDetailsViewModel.AccountItem
            {
                AccountNumber = r.AccountId,
                AccountOwnership = "OWNER",
                Balance = r.Balance,
                CreationDate = Convert.ToDateTime(r.Created).ToString("yyyy-MM-dd"),
                Frequency = r.Frequency
            });

            var result = sut.Details(customer.CustomerId);

            var viewResult = result as ViewResult;
            var model = viewResult.ViewData.Model as CustomerDetailsViewModel;

            Assert.AreEqual(Convert.ToDateTime(customer.Birthday).ToString("yyyy-MM-dd"), model.Birthday);
            Assert.AreEqual(customer.Emailaddress, model.Email);
            Assert.AreEqual("Full address", model.FullAddress);
            Assert.AreEqual("Full name", model.FullName);
            Assert.AreEqual("Telephone number", model.FullTelephoneNumber);
            Assert.AreEqual(customer.Gender, model.Gender);
            Assert.AreEqual(customer.CustomerId, model.Id);
            Assert.AreEqual("National id", model.NationalId);

            var expected = fakeAccountsItems.First();
            var actual = model.AccountItems.First();

            Assert.AreEqual(expected.AccountNumber, actual.AccountNumber);
            Assert.AreEqual(expected.AccountOwnership, actual.AccountOwnership);
            Assert.AreEqual(expected.Balance, actual.Balance);
            Assert.AreEqual(expected.Frequency, actual.Frequency);
            Assert.AreEqual(expected.CreationDate, actual.CreationDate);
        }

        private List<Account> GetAccounts(Customer c)
        {
            return ctx.Accounts.Where(a => a.Dispositions.Any(d => d.CustomerId == c.CustomerId)).ToList();
        }
    }
}