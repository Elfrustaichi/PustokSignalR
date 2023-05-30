using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokBackTask.DAL;
using PustokBackTask.Enums;
using PustokBackTask.Models;
using PustokBackTask.ViewModels;

namespace PustokBackTask.Areas.Manage.Controllers
{
    [Authorize(Roles ="Admin,SuperAdmin")]
    [Area("manage")]
    public class OrderController : Controller
    {
        private readonly DataContext _context;

        public OrderController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            var query = _context.Orders.Include(x => x.OrderItems).AsQueryable();
            var data = PaginatedList<Order>.Create(query, page, 8);

            //var enums=Enum.GetValues(typeof(OrderStatus));

            //Dictionary<byte,string> enumdata=new Dictionary<byte,string>();

            //foreach (var item in enums)
            //{
            //    enumdata.Add((byte)item, item.ToString());
            //}

            //ViewBag.Statues = enumdata;

            return View(data);
        }


        public IActionResult Detail(int id)
        {
            Order order = _context.Orders.Include(x => x.OrderItems).ThenInclude(x => x.Book).FirstOrDefault(x => x.Id == id);

            if (order == null)
                return View("Error");

            return View(order);
        }

        public IActionResult Accept(int id)
        {
            Order order = _context.Orders.FirstOrDefault(x => x.Id == id);

            if (order == null)
                return View("Error");

            order.Status = OrderStatus.Accepted;
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Reject(int id)
        {
            Order order = _context.Orders.FirstOrDefault(x => x.Id == id);

            if (order == null)
                return View("Error");

            order.Status = OrderStatus.Rejected;
            _context.SaveChanges();

            return RedirectToAction("index");
        }
        

    }
}
