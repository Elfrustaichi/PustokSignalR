using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PustokBackTask.DAL;
using PustokBackTask.Models;
using PustokBackTask.ViewModels;
using System.Security.Claims;

namespace PustokBackTask.Controllers
{
    public class OrderController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;
        public OrderController(DataContext context,UserManager<AppUser> userManager)
        {
            _context = context; 
            _userManager = userManager;
        }
        public async Task<IActionResult> Checkout()
        {
            OrderViewModel OrderVM = new OrderViewModel();
            OrderVM.CheckoutItems=GenerateCheckoutItem();
            if (User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

               

                OrderVM.Order = new OrderCreateViewModel
                {
                    Address = user.Adress,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                };

            }
          
            OrderVM.TotalPrice = OrderVM.CheckoutItems.Any() ? OrderVM.CheckoutItems.Sum(x => x.Price * x.Count) : 0;
            return View(OrderVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderCreateViewModel OrderCreateVM)
        {
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Member"))
            {
                if (string.IsNullOrWhiteSpace(OrderCreateVM.FullName))
                {
                    ModelState.AddModelError("FullName", "FullName required");
                }

                if (string.IsNullOrWhiteSpace(OrderCreateVM.Email))
                {
                    ModelState.AddModelError("Email", "Email is required");
                }
            }
            if (!ModelState.IsValid)
            {
                OrderViewModel orderVM = new OrderViewModel();
                orderVM.CheckoutItems = GenerateCheckoutItem();
                orderVM.Order = OrderCreateVM;
                return View("Checkout",orderVM);
            }

            Order order = new Order
            {
                Address = OrderCreateVM.Address,
                Note = OrderCreateVM.Note,
                Phone = OrderCreateVM.Phone,
                Status = Enums.OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow.AddHours(4)
            };
            var items=GenerateCheckoutItem();

            foreach (var item in items)
            {
                Book book = _context.Books.Find(item.BookId);

                OrderItem orderitem = new OrderItem
                {
                    BookId = item.BookId,
                    DiscountPercent = book.DiscountPercent,
                    UnitCostPrice = book.CostPrice,
                    UnitPrice = book.DiscountPercent > 0 ? (book.SalePrice * (100 - book.DiscountPercent) / 100) : book.SalePrice,
                    Count = item.Count,
                };
                order.OrderItems.Add(orderitem);
            }
            if(User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                AppUser user=await _userManager.FindByNameAsync(User.Identity.Name);

                order.FullName= user.FullName;
                order.Email= user.Email;
                order.AppUserId=user.Id;

                ClearDbBasket(user.Id);
            }
            else
            {
                order.FullName = OrderCreateVM.FullName;
                order.Email= OrderCreateVM.Email;
                ClearCookieBasket();
                
            }
            _context.Orders.Add(order);
            _context.SaveChanges();

            TempData["Success"] = "Order Created successfully";
            
            return RedirectToAction("index","home");
        }

        private List<CheckoutItemViewModel> GenerateCheckoutItemsDB(string userId)
        {
            return _context.BasketItems.Include(x => x.Book).Where(x => x.AppUser.Id == userId).Select(x => new CheckoutItemViewModel
            {
                Count = x.Count,
                Name = x.Book.Name,
                BookId= x.Book.Id,
                Price = x.Book.DiscountPercent > 0 ? (x.Book.SalePrice * (100 - x.Book.DiscountPercent) / 100) : x.Book.SalePrice,
            }).ToList();
        }

        private List<CheckoutItemViewModel> GenerateCheckoutItemsCookie()
        {
            List<CheckoutItemViewModel> checkoutItemViewModels = new List<CheckoutItemViewModel>();
            var BasketItems = Request.Cookies["basket"];

            List<BasketCookieViewModel> Cookies = JsonConvert.DeserializeObject<List<BasketCookieViewModel>>(BasketItems);

            foreach (var item in Cookies)
            {
                Book book = _context.Books.FirstOrDefault(x => x.Id == item.BookId);
                CheckoutItemViewModel checkoutitem = new CheckoutItemViewModel
                {
                    Count = item.BookCount,
                    Name = book.Name,
                    BookId= book.Id,
                    Price = book.DiscountPercent > 0 ? (book.SalePrice * (100 - book.DiscountPercent) / 100) : book.SalePrice,
                };
                checkoutItemViewModels.Add(checkoutitem);
            }
            return checkoutItemViewModels;
        }

        private List<CheckoutItemViewModel> GenerateCheckoutItem()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return GenerateCheckoutItemsDB(userId);
            }
            else
                return GenerateCheckoutItemsCookie();
            
        }

        private void ClearDbBasket(string UserId)
        {
            _context.BasketItems.RemoveRange(_context.BasketItems.Where(x => x.AppUserId == UserId).ToList());
            _context.SaveChanges();
        }

        private void ClearCookieBasket()
        {
            Response.Cookies.Delete("Basket");
        }
    }
}
