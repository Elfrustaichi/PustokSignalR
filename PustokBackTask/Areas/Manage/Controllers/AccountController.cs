using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PustokBackTask.Areas.Manage.ViewModels;
using PustokBackTask.Models;

namespace PustokBackTask.Areas.Manage.Controllers
{
    [Authorize(Roles ="SuperAdmin,Admin")]
        [Area("manage")]
        public class AccountController : Controller
        {
            private readonly UserManager<AppUser> _userManager;

        private readonly SignInManager<AppUser> _signInManager;

        private readonly RoleManager<IdentityRole> _roleManager;

            public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,RoleManager<IdentityRole> RoleManager)
            {
                _userManager = userManager;
                _signInManager=signInManager;
                _roleManager=RoleManager;
            }

        //public async Task<IActionResult> CreateAdmin()
        //{
        //    AppUser User = new AppUser
        //    {
        //        UserName = "S-Admin",
        //        IsAdmin = true,

        //    };
        //    var result= await _userManager.CreateAsync(User,"Admin123");
        //    await _userManager.AddToRoleAsync(User, "SuperAdmin");

        //    return Json(result);
        //}
           
        //   public async Task<IActionResult> CreateRoles()
        //{
        //    await _roleManager.CreateAsync(new IdentityRole ("SuperAdmin" ));
        //    await _roleManager.CreateAsync(new IdentityRole ("Admin" ));
        //    await _roleManager.CreateAsync(new IdentityRole ("Member" ));

        //    return Ok();
        //}


            public IActionResult Login()
            {
                return View();
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AdminLoginViewModel adminVM,string ReturnUrl=null)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            AppUser user = await _userManager.FindByNameAsync(adminVM.Username);

            if (user == null || !user.IsAdmin)
            {
                ModelState.AddModelError("", "Username or password is incorrect");
                return View();
            }

            var result=await _signInManager.PasswordSignInAsync(user, adminVM.Password, false,false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username or password is incorrect");
                return View();
            }
            if (ReturnUrl!=null)
            {
                return Redirect(ReturnUrl);
            }
            return RedirectToAction("index","dashboard");
           

        }


     }
}
