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
            var customer = fixture.Create<Customer>();

            ctx.Customers.Add(customer);
            ctx.SaveChanges();

            customerServiceMock.Setup(e => e.GetFullAddress(customer)).Returns(customer.Streetaddress);
            customerServiceMock.Setup(e => e.GetFullName(customer)).Returns(customer.Givenname);
            customerServiceMock.Setup(e => e.GetFullTelephoneNumber(customer)).Returns(customer.Telephonenumber);
            customerServiceMock.Setup(e => e.GetNationalIdOutput(customer)).Returns(customer.NationalId);
            customerServiceMock.Setup(e => e.GetAccounts(customer.CustomerId)).Returns(new List<Account>{new Account{AccountId = 5}});
            customerServiceMock.Setup(e => e.GetAccountOwnershipInfo(It.IsAny<int>(), It.IsAny<int>()))
                .Returns("OWNER");

            var result = sut.Details(customer.CustomerId);

            var viewResult = result as ViewResult;
            var model = viewResult.ViewData.Model as CustomerDetailsViewModel;

            Assert.AreEqual(Convert.ToDateTime(customer.Birthday).ToString("yyyy-MM-dd"), model.Birthday);
            Assert.AreEqual(customer.Emailaddress, model.Email);
            Assert.AreEqual(customer.Streetaddress, model.FullAddress);
            Assert.AreEqual(customer.Givenname, model.FullName);
            Assert.AreEqual(customer.Telephonenumber, model.FullTelephoneNumber);
            Assert.AreEqual(customer.Gender, model.Gender);
            Assert.AreEqual(customer.CustomerId, model.Id);
            Assert.AreEqual(customer.NationalId, model.NationalId);

            foreach (var a in model.AccountItems)
            {
                Assert.AreEqual(a.AccountNumber, 5);
                Assert.AreEqual(a.AccountOwnership, "OWNER");
                Assert.AreEqual(a.Balance, 0);
                Assert.AreEqual(a.Frequency, null);
                Assert.AreEqual(a.CreationDate, "0001-01-01");
            }
        }

        //private List<Account> GetAccounts(Customer c)
        //{
        //    return ctx.Accounts.Where(a => a.Dispositions.Any(d => d.CustomerId == c.CustomerId)).ToList();
        //}
    }
}