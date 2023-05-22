using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PustokBackTask.Models;

namespace PustokBackTask.Areas.Manage.Controllers
{

        [Area("manage")]
        public class AccountController : Controller
        {
            private readonly UserManager<AppUser> _userManager;

            public AccountController(UserManager<AppUser> userManager)
            {
                _userManager = userManager;
            }
           
            


            public IActionResult Login()
            {
                return View();
            }
        }
}
