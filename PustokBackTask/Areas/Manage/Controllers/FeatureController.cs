using Microsoft.AspNetCore.Mvc;
using PustokBackTask.DAL;
using PustokBackTask.Models;
using PustokBackTask.ViewModels;

namespace PustokBackTask.Areas.Manage.Controllers
{
    [Area("manage")]
    public class FeatureController : Controller
    {
        private readonly DataContext _context;
        public FeatureController(DataContext context)
        {
            _context=context;
        }
        public IActionResult Index(int page=1)
        {
            var query = _context.Features.AsQueryable();

           
            
            return View(PaginatedList<Feature>.Create(query,page,3));
        }
    }
}
