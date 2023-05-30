using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PustokBackTask.Areas.Manage.ViewModels;
using PustokBackTask.Models;

namespace PustokBackTask.Areas.Manage.Controllers
{
   
        [Area("manage")]
        public class AccountController : Controller
        {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AdminLoginViewModel adminVM, string returnUrl = null)
        {
            AppUser user = await _userManager.FindByNameAsync(adminVM.Username);

            if (user == null || !user.IsAdmin)
            {
                ModelState.AddModelError("", "UserName or Password incorrect");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user, adminVM.Password, false, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "UserName or Password incorrect");
                return View();
            }

            if (returnUrl != null) return Redirect(returnUrl);

            return RedirectToAction("index", "dashboard");
        }

        public IActionResult ShowUser()
        {
            return Json(new
            {
                isAuthenticated = User.Identity.IsAuthenticated,
            });
        }


    }


     
}
