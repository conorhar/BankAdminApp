using System;
using AutoFixture;
using BankAdminApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharedThings.Data;
using SharedThings.Models;
using SharedThings.Services.Customers;
using SharedThings.ViewModels;

namespace BankAdminApp.Tests
{
    [TestClass]
    public class CountryControllerTests : BaseTest
    {
        private CountryController sut;
        private Mock<ICustomerService> customerServiceMock;
        private ApplicationDbContext ctx;

        public CountryControllerTests()
        {
            customerServiceMock = new Mock<ICustomerService>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;
            ctx = new ApplicationDbContext(options);

            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            sut = new CountryController(ctx, customerServiceMock.Object);
        }

        [TestMethod]
        public void WhenTopTenIsCalledAndCountryExistsInDatabaseMaxTenCustomersShouldBeReturned()
        {
            var customers = fixture.Build<Customer>()
                .With(x => x.Country, "test country")
                .CreateMany<Customer>(10);
            
            ctx.AddRange(customers);
            ctx.SaveChanges();

            customerServiceMock.Setup(e => e.GetFullName(It.IsAny<Customer>())).Returns("Full name");

            var result = sut.TopTen("test country");
            var viewResult = result as ViewResult;
            var model = viewResult.ViewData.Model as CountryTopTenViewModel;

            Assert.IsTrue(model.CustomerItems.Count <= 10);
        }


        [TestMethod]
        public void WhenTopTenIsCalledGetFullNameShouldBeCalledSameAmountTimesAsAmountCustomersReturned()
        {
            int amountCustomers = 10;

            var customers = fixture.Build<Customer>()
                .With(x => x.Country, "test country")
                .CreateMany<Customer>(amountCustomers);

            ctx.AddRange(customers);
            ctx.SaveChanges();

            customerServiceMock.Setup(e => e.GetFullName(It.IsAny<Customer>())).Returns("Full name");

            sut.TopTen("test country");

            customerServiceMock.Verify(e => e.GetFullName(It.IsAny<Customer>()), Times.Exactly(amountCustomers));
        }

        [TestMethod]
        public void WhenTopTenIsCalledCountryNameInViewModelShouldMatchInParameter()
        {
            var customer = fixture.Build<Customer>()
                .With(x => x.Country, "test country")
                .Create<Customer>();

            ctx.Add(customer);
            ctx.SaveChanges();

            customerServiceMock.Setup(e => e.GetFullName(It.IsAny<Customer>())).Returns("Full name");

            var result = sut.TopTen("test country");
            var viewResult = result as ViewResult;
            var model = viewResult.ViewData.Model as CountryTopTenViewModel;

            Assert.AreEqual("test country", model.Country);
        }
    }
}