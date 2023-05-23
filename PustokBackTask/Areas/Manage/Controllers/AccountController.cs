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

            public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager)
            {
                _userManager = userManager;
                _signInManager=signInManager;
            }

        //public async Task<IActionResult> CreateAdmin()
        //{
        //    AppUser User = new AppUser
        //    {
        //        UserName = "admin",
        //        IsAdmin = true,

        //    };
        //    var result= await _userManager.CreateAsync(User,"Admin333");
        //    return Json(result);
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
