using Microsoft.AspNetCore.Mvc;

namespace BankAdminApp.Controllers
{
    public class TransactionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}