using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SharedThings;
using SharedThings.Models;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public CustomerController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Customer>> Get()
        {
            return Ok(_dbContext.Customers);
        }

        [Route("{id}")]
        [HttpGet]
        public ActionResult<Customer> GetSingle(int id)
        {
            var customer = _dbContext.Customers.FirstOrDefault(e => e.CustomerId == id);
            if (customer == null) return NotFound();
            
            return Ok(customer);
        }
    }
}