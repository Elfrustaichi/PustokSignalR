using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Newtonsoft.Json;
using PustokBackTask.DAL;
using PustokBackTask.Models;
using PustokBackTask.ViewModels;
using System.Collections.Generic;

namespace PustokBackTask.Controllers
{
    public class BookController : Controller
    {
        private readonly DataContext _context;
        public BookController(DataContext context)
        {
            _context = context;
        }
        public IActionResult BookDetail(int id)
        {
            Book book =_context.Books
                .Include(x=>x.Author)
                .Include(x=>x.BookImages)
                .Include(x=>x.BookTags).ThenInclude(x=>x.Tag)
                .FirstOrDefault(x=>x.Id==id);

            return PartialView("_BookModalPartial",book);
        }

        public IActionResult AddToBasket(int id) 
        {
            List<BasketCookieViewModel> BasketItems= new List<BasketCookieViewModel>();
            BasketCookieViewModel CookieItem;
            var basketString = Request.Cookies["basket"];
            if (basketString != null)
            {
                BasketItems = JsonConvert.DeserializeObject<List<BasketCookieViewModel>>(basketString);

                CookieItem = BasketItems.FirstOrDefault(x=>x.BookId==id);

                if (CookieItem!=null)
                {
                    CookieItem.BookCount++;
                }
                else
                {
                    CookieItem = new BasketCookieViewModel { BookId=id,BookCount=1};
                    BasketItems.Add(CookieItem);
                }
            }
            else
            {
                CookieItem = new BasketCookieViewModel{BookId = id, BookCount = 1 };
                BasketItems.Add(CookieItem);
            }


            Response.Cookies.Append("Basket", JsonConvert.SerializeObject(BasketItems));

            BasketViewModel bv = new BasketViewModel();
            foreach (var ci in BasketItems)
            {
                BasketItemViewModel bi = new BasketItemViewModel()
                {
                    Count = ci.BookCount,
                    Book = _context.Books.Include(x => x.BookImages).FirstOrDefault(x => x.Id == ci.BookId)
                };
                bv.BasketItems.Add(bi);
                bv.TotalPrice += (bi.Book.DiscountPercent > 0 ? (bi.Book.SalePrice * (100 - bi.Book.DiscountPercent) / 100) : bi.Book.SalePrice) * bi.Count;
            }
            return PartialView("_BasketItemPartialView",bv);
        }

       public IActionResult RemoveItemFromBasket(int id)
        {
            var basketStr = Request.Cookies["basket"];
            if (basketStr == null)
                return StatusCode(404);

            List<BasketCookieViewModel> cookieItems = JsonConvert.DeserializeObject<List<BasketCookieViewModel>>(basketStr);

            BasketCookieViewModel item = cookieItems.FirstOrDefault(x => x.BookId == id);

            if (item == null)
                return StatusCode(404);

            if (item.BookCount > 1)
                item.BookCount--;
            else
                cookieItems.Remove(item);

            Response.Cookies.Append("basket", JsonConvert.SerializeObject(cookieItems));

            BasketViewModel bv = new BasketViewModel();
            foreach (var ci in cookieItems)
            {
                BasketItemViewModel bi = new BasketItemViewModel
                {
                    Count = ci.BookCount,
                    Book = _context.Books.Include(x => x.BookImages).FirstOrDefault(x => x.Id == ci.BookId)
                };
                bv.BasketItems.Add(bi);
                bv.TotalPrice += (bi.Book.DiscountPercent > 0 ? (bi.Book.SalePrice * (100 - bi.Book.DiscountPercent) / 100) : bi.Book.SalePrice) * bi.Count;
            }

            return PartialView("_BasketItemPartialView", bv);

        }
    }
}
