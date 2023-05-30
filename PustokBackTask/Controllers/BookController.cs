using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Newtonsoft.Json;
using PustokBackTask.DAL;
using PustokBackTask.Models;
using PustokBackTask.ViewModels;
using System.Collections.Generic;
using System.Security.Claims;

namespace PustokBackTask.Controllers
{
    public class BookController : Controller
    {
        private readonly DataContext _context;
        public BookController(DataContext context)
        {
            _context = context;
        }

		public IActionResult Detail(int id)
		{
			Book book = _context.Books
				.Include(x => x.BookImages)
				.Include(x => x.Author)
				.Include(x => x.Genre)
				.Include(x => x.BookComments).ThenInclude(x => x.AppUser)
				.Include(x => x.BookTags).ThenInclude(bt => bt.Tag).FirstOrDefault(x => x.Id == id);

			if (book == null) return View("Error");

			BookDetailViewModel vm = new BookDetailViewModel
			{
				Book = book,
				RelatedBooks = _context.Books.Include(x => x.BookImages).Where(x => x.GenreId == book.GenreId).ToList(),
				Comment = new BookComment { BookId = id }
			};



			return View(vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Comment(BookComment comment)
		{

			if (!User.Identity.IsAuthenticated || !User.IsInRole("Member"))
				return RedirectToAction("login", "account", new { returnUrl = Url.Action("detail", "book", new { id = comment.BookId }) });


			if (!ModelState.IsValid)
			{
				
				Book book = _context.Books
			.Include(x => x.BookImages)
			.Include(x => x.Author)
			.Include(x => x.Genre)
				.Include(x => x.BookComments).ThenInclude(x => x.AppUser)
			.Include(x => x.BookTags).ThenInclude(bt => bt.Tag).FirstOrDefault(x => x.Id == comment.BookId);

				if (book == null) return View("Error");

				BookDetailViewModel vm = new BookDetailViewModel
				{
					Book = book,
					RelatedBooks = _context.Books.Include(x => x.BookImages).Where(x => x.GenreId == book.GenreId).ToList(),
					Comment = new BookComment { BookId = comment.BookId }
				};
				vm.Comment = comment;
				return View("Detail", vm);
			}

			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			comment.AppUserId = userId;
			comment.CreatedAt = DateTime.UtcNow.AddHours(4);
			_context.BookComments.Add(comment);
			_context.SaveChanges();

			return RedirectToAction("detail", new { id = comment.BookId });

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
            if (User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);


				var basketItem=_context.BasketItems.FirstOrDefault(x=>x.BookId==id && x.AppUserId==UserId);

                if (basketItem!=null)
                    basketItem.Count++;
                else
                {
                    basketItem = new BasketItem
                    {
                        AppUserId=User.FindFirstValue(ClaimTypes.NameIdentifier),
                        BookId=id,
                        Count=1
                    };
					_context.BasketItems.Add(basketItem);
                   
				}
				_context.SaveChanges();
                var BasketItems = _context.BasketItems.Include(x=>x.Book).ThenInclude(x=>x.BookImages).Where(x => x.AppUserId == UserId).ToList();

				
				return PartialView("_BasketItemPartialView", GenerateBasketVM(BasketItems));




			}
            else
            {
				List<BasketCookieViewModel> BasketItems = new List<BasketCookieViewModel>();

				BasketCookieViewModel CookieItem;
				var basketString = Request.Cookies["basket"];
				if (basketString != null)
				{
					BasketItems = JsonConvert.DeserializeObject<List<BasketCookieViewModel>>(basketString);

					CookieItem = BasketItems.FirstOrDefault(x => x.BookId == id);

					if (CookieItem != null)
					{
						CookieItem.BookCount++;
					}
					else
					{
						CookieItem = new BasketCookieViewModel { BookId = id, BookCount = 1 };
						BasketItems.Add(CookieItem);
					}
				}
				else
				{
					CookieItem = new BasketCookieViewModel { BookId = id, BookCount = 1 };
					BasketItems.Add(CookieItem);
				}


				Response.Cookies.Append("Basket", JsonConvert.SerializeObject(BasketItems));

				
				return PartialView("_BasketItemPartialView", GenerateBasketVM(BasketItems));
			}
			

		}

		private BasketViewModel GenerateBasketVM(List<BasketCookieViewModel> BasketItems)
		{
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
			return bv;
		}

		private BasketViewModel GenerateBasketVM(List<BasketItem> BasketItems)
		{
			BasketViewModel bv = new BasketViewModel();
			foreach (var bi in BasketItems)
			{
				BasketItemViewModel bivm = new BasketItemViewModel()
				{
					Count = bi.Count,
					Book = bi.Book,
				};
				bv.BasketItems.Add(bivm);
				bv.TotalPrice += (bivm.Book.DiscountPercent > 0 ? (bi.Book.SalePrice * (100 - bi.Book.DiscountPercent) / 100) : bi.Book.SalePrice) * bi.Count;
			}
			return bv;
		}

	   public IActionResult RemoveItemFromBasket(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
				string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

				var BasketItem= _context.BasketItems.FirstOrDefault(x => x.BookId == id && x.AppUserId==UserId);

				if (BasketItem == null)
				 return StatusCode(404);
				

				if (BasketItem.Count > 1)
				{
					BasketItem.Count--;
				}
				else
				{
					_context.BasketItems.Remove(BasketItem);

					
				}

				_context.SaveChanges();

				var BasketItems = _context.BasketItems.Include(x => x.Book).ThenInclude(x => x.BookImages).Where(x => x.AppUserId == UserId).ToList();

				BasketViewModel bv = new BasketViewModel();
				foreach (var bi in BasketItems)
				{
					BasketItemViewModel bivm = new BasketItemViewModel()
					{
						Count = bi.Count,
						Book = bi.Book,
					};
					bv.BasketItems.Add(bivm);
					bv.TotalPrice += (bivm.Book.DiscountPercent > 0 ? (bi.Book.SalePrice * (100 - bi.Book.DiscountPercent) / 100) : bi.Book.SalePrice) * bi.Count;
				}
				return PartialView("_BasketItemPartialView", bv);




			}
			else
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
}
