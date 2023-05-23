using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokBackTask.DAL;
using PustokBackTask.Helpers;
using PustokBackTask.Models;
using PustokBackTask.ViewModels;

namespace PustokBackTask.Areas.Manage.Controllers
{
    [Authorize]
    [Area("manage")]
    public class AuthorController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;
        public AuthorController(DataContext context, IWebHostEnvironment env) 
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page=1,string search=null)
        {
            var query= _context.Authors.Include(x => x.Books).AsQueryable();

            if (search!=null)
            {
                query=query.Where(x=>x.FullName.Contains(search));
            }
            ViewBag.Search = search;

            return View(PaginatedList<Author>.Create(query,page,3));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Author author)
        {
          
            

            _context.Authors.Add(author);
            _context.SaveChanges();


            return RedirectToAction("index");
        }

       

        public IActionResult Edit(int id)
        {
            Genre genre = _context.Genres.Find(id);

            return View(genre);
        }

        [HttpPost]
        public IActionResult Edit(Author author)
        {

            Author ExistAuthor = _context.Authors.Find(author.Id);

            ExistAuthor.FullName = author.FullName;

            _context.SaveChanges();
            return RedirectToAction("index");

        }

        public IActionResult Delete(int id)
        {
            Author author = _context.Authors.Include(x => x.Books).FirstOrDefault(x => x.Id == id);


            return View(author);
        }

        [HttpPost]
        public IActionResult Delete(Author author)
        {
            Author ExistAuthor = _context.Authors.Find(author.Id);

            

            _context.Authors.Remove(ExistAuthor);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
    }
}
