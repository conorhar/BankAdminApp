using System.Linq;
using BankAdminApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharedThings.Data;
using SharedThings.Services.Users;

namespace BankAdminApp.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserService _userService;

        public UserController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager, IUserService userService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _userService = userService;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var viewModel = new UserIndexViewModel();

            viewModel.Users = _dbContext.Users.Select(dbUser => new UserIndexViewModel.UserItem
            {
                Id = dbUser.Id,
                UserName = dbUser.UserName
            }).ToList();

            foreach (var user in viewModel.Users)
            {
                user.Role = _userService.FindRoleName(user.Id);
            }
            
            return View(viewModel);
        }
    }
}