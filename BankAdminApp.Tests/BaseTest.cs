using System;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using SharedThings.Data;

namespace BankAdminApp.Tests
{
    public class BaseTest
    {
        protected Fixture fixture = new Fixture();
        private ApplicationDbContext ctx;
    }
}