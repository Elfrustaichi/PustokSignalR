using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using PustokBackTask.DAL;
using PustokBackTask.Models;
using PustokBackTask.ViewModels;
using System.Security.Claims;

namespace PustokBackTask.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly DataContext _context;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,DataContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(MemberLoginViewModel MemberLoginVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            AppUser user=await _userManager.FindByEmailAsync(MemberLoginVM.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Email or password is incorrect");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user,MemberLoginVM.Password, false,true);

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "So many False password tries");
                return View();
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Email or password is incorrect");
                return View();
            }
            
            return RedirectToAction("index", "home");
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(MemberRegisterViewModel registerVM)
        {
            if (!ModelState.IsValid) return View();

            if (_userManager.Users.Any(x => x.UserName == registerVM.UserName))
            {
                ModelState.AddModelError("UserName", "UserName is alredy taken");
                return View();
            }

            if (_userManager.Users.Any(x => x.Email == registerVM.Email))
            {
                ModelState.AddModelError("Email", "Email is alredy taken");
                return View();
            }

            AppUser user = new AppUser
            {
                FullName = registerVM.FullName,
                UserName = registerVM.UserName,
                Email = registerVM.Email,
               
                IsAdmin = false
            };

            var result = await _userManager.CreateAsync(user, registerVM.Password);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError("", err.Description);
                return View();
            }

            await _userManager.AddToRoleAsync(user, "Member");

            await _signInManager.SignInAsync(user, false);

            return RedirectToAction("index", "home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("login", "account");
        }

        [Authorize(Roles ="Member")]
        public async Task<IActionResult> Profile()
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
            {
                _signInManager.SignOutAsync();
                return RedirectToAction("login");
            }

            AccountProfileViewModel ApVm = new AccountProfileViewModel
            {
                Profile = new ProfileEditViewModel
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    UserName = user.UserName,
                    Phone=user.Phone,
                    Address=user.Adress
                },
                Orders=_context.Orders.Include(x=>x.OrderItems).ThenInclude(x=>x.Book).Where(x=>x.AppUserId==user.Id).ToList(),

            };

            return View(ApVm);
        }
        [Authorize(Roles ="Member")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileEditViewModel profileVM)
        {
            if(!ModelState.IsValid)
            {
                AccountProfileViewModel ApVm = new AccountProfileViewModel
                {
                    Profile = profileVM
                    

                };
                return View(ApVm);
            }

            AppUser user= await _userManager.FindByNameAsync(User.Identity.Name);

            user.FullName= profileVM.FullName;
            user.Email= profileVM.Email;
            user.UserName= profileVM.UserName;
            user.Phone= profileVM.Phone;
            user.Adress = profileVM.Address;
            var result=await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                AccountProfileViewModel ApVm = new AccountProfileViewModel
                {
                    Profile = profileVM


                };
                return View(ApVm);
            }

            await _signInManager.SignInAsync(user,false);

            return RedirectToAction("profile");
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel passwordVM)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = await _userManager.FindByEmailAsync(passwordVM.Email);

            if (user == null || user.IsAdmin)
            {
                ModelState.AddModelError("Email", "Email is not correct");
                return View();
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

            string url = Url.Action("resetpassword", "account", new { email = passwordVM.Email, token = token }, Request.Scheme);

            return Json(new { url = url });
        }

        public async Task<IActionResult> Resetpassword(string email, string token)
        {
            AppUser user = await _userManager.FindByEmailAsync(email);

            if (user == null || user.IsAdmin || !await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token))
                return RedirectToAction("login");

            ViewBag.Email = email;
            ViewBag.Token = token;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetVM)
        {
            AppUser user = await _userManager.FindByEmailAsync(resetVM.Email);

            if (user == null || user.IsAdmin)
                return RedirectToAction("login");

            var result = await _userManager.ResetPasswordAsync(user, resetVM.Token, resetVM.Password);

            if (!result.Succeeded)
            {
                return RedirectToAction("login");
            }

            return RedirectToAction("login");
        }

    }
}
