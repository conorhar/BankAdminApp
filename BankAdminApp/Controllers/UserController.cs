using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharedThings.Data;
using SharedThings.Services.Users;
using SharedThings.ViewModels;

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

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(string id)
        {
            var viewModel = new UserEditViewModel();

            var dbUser = _dbContext.Users.First(r => r.Id == id);

            viewModel.ReferenceList = _userService.GetRoleReference();

            viewModel.Id = dbUser.Id;
            viewModel.UserName = dbUser.UserName;
            viewModel.AllRoles = _userService.GetRolesListItems();
            var role = _dbContext.Roles.FirstOrDefault(r => r.Name == _userService.FindRoleName(id));
            viewModel.SelectedRoleId = viewModel.ReferenceList.First(r => r.DatabaseId == role.Id).SelectBoxId;

            return View(viewModel);
        }
    }
}