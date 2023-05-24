using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PustokBackTask.DAL;
using PustokBackTask.Models;
using PustokBackTask.ViewModels;
using System.Security.Claims;

namespace PustokBackTask.Services
{
    public class LayoutService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

      

       public LayoutService(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public Dictionary<string,string> GetSettings()
        {
            return _context.Settings.ToDictionary(x=>x.Key,x=>x.Value);
        }

        public List<Genre> GetGenres()
        {
            return _context.Genres.ToList();
        }
        public BasketViewModel GetBasket()
        {
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated &&_httpContextAccessor.HttpContext.User.IsInRole("Member"))
            {
				var BasketItems=_context.BasketItems.Include(x=>x.Book).ThenInclude(x=>x.BookImages).Where(x=>x.AppUserId==_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList();
                var bv = new BasketViewModel();
				

				foreach (var bi in BasketItems)
				{
					BasketItemViewModel bivm = new BasketItemViewModel
					{
						Count = bi.Count,
						Book = bi.Book
					};
					bv.BasketItems.Add(bivm);
					bv.TotalPrice += (bi.Book.DiscountPercent > 0 ? (bi.Book.SalePrice * (100 - bi.Book.DiscountPercent) / 100) : bi.Book.SalePrice) * bi.Count;
				}
				return bv;

			}
            else
            {
				var bv = new BasketViewModel();
				var basketJson = _httpContextAccessor.HttpContext.Request.Cookies["basket"];

				if (basketJson != null)
				{
					var cookieItems = JsonConvert.DeserializeObject<List<BasketCookieViewModel>>(basketJson);

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
				}

				return bv;
			}
           
        }
    }
    
}
