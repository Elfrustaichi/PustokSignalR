using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokBackTask.DAL;
using PustokBackTask.Models;
using PustokBackTask.ViewModels;

namespace PustokBackTask.Areas.Manage.Controllers
{
    [Area("manage")]
    public class TagController : Controller
    {
        private readonly DataContext _context;
        public TagController(DataContext context)
        {
            _context=context;
        }
        public IActionResult Index(int page=1,string search=null)
        {
            var query=_context.Tags.Include(x=>x.BookTags).AsQueryable();

            if(search != null)
            {
                query=query.Where(x=>x.Name.Contains(search));
            }

            ViewBag.Search = search;
            return View(PaginatedList<Tag>.Create(query,page,3));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Tag tag)
        {
            _context.Tags.Add(tag);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            Tag tag = _context.Tags.Find(id);

            return View(tag);
        }

        [HttpPost]
        public IActionResult Edit(Tag tag)
        {

            Tag ExistTag = _context.Tags.Find(tag.Id);

            ExistTag.Name = tag.Name;

            _context.SaveChanges();
            return RedirectToAction("index");

        }

        public IActionResult Delete(int id)
        {
            Tag tag = _context.Tags.Include(x => x.BookTags).FirstOrDefault(x => x.Id == id);


            return View(tag);
        }

        [HttpPost]
        public IActionResult Delete(Tag tag)
        {
            Tag ExistTag = _context.Tags.Find(tag.Id);


            _context.Tags.Remove(ExistTag);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
    }
}
