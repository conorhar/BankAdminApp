using System;
using System.Collections.Generic;
using System.Linq;
using BankAdminApp.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BankAdminApp.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public TransactionController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AutoComplete(string prefix)
        {

            if (int.TryParse(prefix, out int n))
            {
                var customers = (from c in _dbContext.Customers
                    where c.CustomerId == n
                    select new
                    {
                        label = $"{c.Givenname} {c.Surname} ({c.CustomerId})",
                        val = c.CustomerId
                    }).ToList();

                return Json(customers);
            }
            else
            {
                var customers = (from c in _dbContext.Customers
                    where c.Givenname.StartsWith(prefix) || c.Surname.StartsWith(prefix)
                    select new
                    {
                        label = $"{c.Givenname} {c.Surname} (id: {c.CustomerId})",
                        val = c.CustomerId
                    }).ToList();

                return Json(customers);
            }
        }
    }
}