using Microsoft.AspNetCore.Mvc;

namespace BankAdminApp.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}