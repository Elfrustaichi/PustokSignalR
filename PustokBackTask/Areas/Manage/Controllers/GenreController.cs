using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokBackTask.DAL;
using PustokBackTask.Models;
using PustokBackTask.ViewModels;

namespace PustokBackTask.Areas.Manage.Controllers
{
    [Area("manage")]
    public class GenreController : Controller
    {
        private readonly DataContext _context;
        public GenreController(DataContext context)
        {
            _context=context;
        }
        public IActionResult Index(int page=1,string search=null)
        {
            var query = _context.Genres.Include(x=>x.Books).AsQueryable();
            if (search != null)
            {
                query=query.Where(x=>x.Name.Contains(search));
            }

            ViewBag.Search = search;
            return View(PaginatedList<Genre>.Create(query,page,3));
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Genre genre)
        {
            _context.Genres.Add(genre);
            _context.SaveChanges();


            return RedirectToAction("Index");
        }


       public IActionResult Edit(int id)
        {
            Genre genre = _context.Genres.Find(id);

            return View(genre); 
        }

        [HttpPost]
        public IActionResult Edit(Genre genre)
        {

            Genre ExistGenre = _context.Genres.Find(genre.Id);

            ExistGenre.Name= genre.Name;

            _context.SaveChanges();
            return RedirectToAction("index");

        }

        public IActionResult Delete(int id)
        {
            Genre genre = _context.Genres.Include(x => x.Books).FirstOrDefault(x => x.Id == id);

            
            return View(genre);
        }

        [HttpPost]
        public IActionResult Delete(Genre genre)
        {
            Genre existGenre = _context.Genres.Find(genre.Id);

            if (existGenre == null) return View("Error");

            _context.Genres.Remove(existGenre);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
    }
}
