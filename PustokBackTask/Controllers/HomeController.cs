using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokBackTask.DAL;
using PustokBackTask.ViewModels;

namespace PustokBackTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _context;
        public HomeController(DataContext context) 
        { 
            _context = context;
        }

        public IActionResult Index()
        {
            HomeViewModel ViewModel = new HomeViewModel() 
            { 
                features=_context.Features.ToList(),
                sliders=_context.Sliders.ToList(),
                FeaturedBooks=_context.Books.Include(x=>x.Author).Include(x=>x.BookImages).Where(x=>x.IsFeatured).Take(10).ToList(),
                NewBooks=_context.Books.Include(x => x.Author).Include(x => x.BookImages).Where(x=>x.IsNew).Take(10).ToList(),
                DiscountedBooks=_context.Books.Include(x => x.Author).Include(x => x.BookImages).Where(x=>x.DiscountPercent>0).Take(10).ToList(),
            };



            return View(ViewModel);
        }

        
    }
}
