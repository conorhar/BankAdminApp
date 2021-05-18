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
        private readonly SignInManager<IdentityUser> _signInManager;

        public UserController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager, IUserService userService,
            SignInManager<IdentityUser> signInManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _userService = userService;
            _signInManager = signInManager;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index(string username)
        {
            var viewModel = new UserIndexViewModel { Username = username };

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
        public IActionResult New()
        {
            var viewModel = new UserNewViewModel();
            viewModel.AllRoles = _userService.GetRolesListItems();

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult New(UserNewViewModel viewModel)
        {
            viewModel.ReferenceList = _userService.GetRoleReference();

            if (ModelState.IsValid)
            {
                var identityUser = new IdentityUser
                {
                    UserName = viewModel.UserName,
                    Email = viewModel.UserName,
                    EmailConfirmed = true
                };
                var result = _userManager.CreateAsync(identityUser, viewModel.Password).Result;

                _userService.AssignNewUserToRole(viewModel, identityUser);

                return RedirectToAction("Index");
            }

            viewModel.AllRoles = _userService.GetRolesListItems();
            viewModel.ReferenceList = _userService.GetRoleReference();
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
            viewModel.SelectedRoleId = role == null ? 0 : viewModel.ReferenceList.First(r => r.DatabaseId == role.Id).SelectBoxId;

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Edit(string id, UserEditViewModel viewModel)
        {
            viewModel.ReferenceList = _userService.GetRoleReference();

            var user = _userManager.FindByIdAsync(id).Result;
            string oldRoleName = _userService.FindRoleName(id);
            string newRoleName = _userService.GetNewRoleName(viewModel);
            
            if (ModelState.IsValid)
            {
                user.Email = viewModel.UserName;
                user.UserName = viewModel.UserName;

                var updateUser = _userManager.UpdateAsync(user).Result;

                if (oldRoleName != "No role assigned" || newRoleName == "No role assigned")
                {
                    var oldResult = _userManager.RemoveFromRoleAsync(user, oldRoleName).Result;
                }
                if (newRoleName != "No role assigned")
                {
                    var newResult = _userManager.AddToRoleAsync(user, newRoleName).Result;
                }
                
                return RedirectToAction("Index");
            }

            viewModel.AllRoles = _userService.GetRolesListItems();
            viewModel.ReferenceList = _userService.GetRoleReference();
            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ChangePassword(string id)
        {
            var viewModel = new UserChangePasswordViewModel
            {
                Username = _userManager.FindByIdAsync(id).Result.UserName
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult ChangePassword(UserChangePasswordViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var user = _userManager.FindByEmailAsync(viewModel.Username).Result;
            var changePasswordResult = _userManager.ChangePasswordAsync(user, viewModel.OldPassword, viewModel.NewPassword).Result;
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError("OldPassword", error.Description);
                }
                return View(viewModel);
            }

            _signInManager.RefreshSignInAsync(user);
            return RedirectToAction("Index", new { username = viewModel.Username });
        }

        [HttpGet]
        public IActionResult ValidateUsernameIsNew(string username, string id)
        {
            var oldUser = _userManager.FindByIdAsync(id).Result;
            var userFromNewInput = _userManager.FindByEmailAsync(username).Result;

            if (userFromNewInput == null || oldUser.UserName == username)
                return Json(true);
            
            return Json("Username already exists");
        }
    }
}