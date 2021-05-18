using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SharedThings;
using SharedThings.Data;
using SharedThings.Services.Customers;
using SharedThings.ViewModels;
using Swashbuckle.AspNetCore.Annotations;

namespace BankApi.Controllers
{
    [EnableCors("AllowAll")]
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes =
         JwtBearerDefaults.AuthenticationScheme)]
    public class MeController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICustomerService _customerService;

        public MeController(ApplicationDbContext dbContext, ICustomerService customerService)
        {
            _dbContext = dbContext;
            _customerService = customerService;
        }
        
        [HttpGet]
        [SwaggerOperation(OperationId = "GetDetails")]
        [Authorize(Roles = "Customer")]
        public ActionResult<CustomerDetailsViewModel> GetDetails()
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var customer = _dbContext.Customers.FirstOrDefault(e => e.CustomerId == Convert.ToInt32(customerId));
            if (customer == null) return NotFound();

            var model = new SharedThings.ViewModels.CustomerDetailsViewModel
            {
                FullName = _customerService.GetFullName(customer),
                Id = customer.CustomerId,
                FullAddress = _customerService.GetFullAddress(customer),
                Birthday = Convert.ToDateTime(customer.Birthday).ToString("yyyy-MM-dd"),
                Gender = customer.Gender,
                NationalId = _customerService.GetNationalIdOutput(customer),
                FullTelephoneNumber = _customerService.GetFullTelephoneNumber(customer),
                Email = customer.Emailaddress,

                AccountItems = _customerService.GetAccounts(customer.CustomerId).Select(r => new CustomerDetailsViewModel.AccountItem
                {
                    AccountNumber = r.AccountId,
                    Balance = r.Balance,
                    CreationDate = r.Created.ToString("yyyy-MM-dd"),
                    Frequency = r.Frequency,
                    AccountOwnership = _customerService.GetAccountOwnershipInfo(customer.CustomerId, r.AccountId)
                }).ToList()
            };

            model.TotalBalance = model.AccountItems.Sum(r => r.Balance);

            return Ok(model);
        }
    }
}