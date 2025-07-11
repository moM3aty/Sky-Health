using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sky_Health.Data;
using Sky_Health.Models;
using Sky_Health.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sky_Health.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var salesData = new Dictionary<string, decimal>();
            for (int i = 6; i >= 0; i--)
            {
                var date = DateTime.Today.AddDays(-i);
                salesData[date.ToString("dd MMM")] = 0;
            }

            var recentSales = await _context.Orders
                .Where(o => o.OrderDate >= DateTime.Today.AddDays(-6) && (o.OrderStatus == "مدفوع" || o.OrderStatus == "تم التوصيل"))
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new { Date = g.Key, Total = g.Sum(o => o.TotalAmount) })
                .ToListAsync();

            foreach (var sale in recentSales)
            {
                salesData[sale.Date.ToString("dd MMM")] = sale.Total;
            }

            var topCategories = await _context.OrderItems
                .Include(oi => oi.Product.Category)
                .GroupBy(oi => oi.Product.Category.Name)
                .Select(g => new { CategoryName = g.Key, Count = g.Sum(oi => oi.Quantity) })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            var latestOrdersForActivity = await _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .Take(3)
                .Select(o => new ActivityLog
                {
                    Description = $"طلب جديد برقم #{o.Id} من {o.CustomerName}",
                    Timestamp = o.OrderDate,
                    IconClass = "fas fa-receipt",
                    IconBgClass = "bg-primary"
                })
                .ToListAsync();

            var latestProductsForActivity = await _context.Products
                .OrderByDescending(p => p.Id)
                .Take(2)
                .Select(p => new ActivityLog
                {
                    Description = $"تمت إضافة منتج جديد: {p.Name}",
                    Timestamp = DateTime.Now,
                    IconClass = "fas fa-box-open",
                    IconBgClass = "bg-success"
                })
                .ToListAsync();

            var recentActivities = latestOrdersForActivity
                .Concat(latestProductsForActivity)
                .OrderByDescending(a => a.Timestamp)
                .Take(5)
                .ToList();


            var viewModel = new DashboardViewModel
            {
                TotalSales = await _context.Orders.Where(o => o.OrderStatus == "مدفوع" || o.OrderStatus == "تم التوصيل").SumAsync(o => o.TotalAmount),
                NewOrdersCount = await _context.Orders.CountAsync(o => o.OrderStatus == "جديد"),
                ProductsCount = await _context.Products.CountAsync(),
                CategoriesCount = await _context.Categories.CountAsync(),
                LatestOrders = await _context.Orders.OrderByDescending(o => o.OrderDate).Take(5).ToListAsync(),
                RecentActivities = recentActivities,
                SalesChartLabels = salesData.Keys.ToList(),
                SalesChartData = salesData.Values.ToList(),
                TopCategoriesChartLabels = topCategories.Select(c => c.CategoryName).ToList(),
                TopCategoriesChartData = topCategories.Select(c => c.Count).ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetNewOrders()
        {
          
            var newOrders = await _context.Orders
                                        .Where(o => o.OrderStatus == "جديد")
                                        .OrderByDescending(o => o.Id)
                                        .Take(5) 
                                        .Select(o => new { o.Id, o.CustomerName })
                                        .ToListAsync();

            return Json(newOrders);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (username?.ToLower() == "admin" && password == "SkyHealth@2025")
            {
                var claims = new System.Collections.Generic.List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "Admin")
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return RedirectToAction("Index", "Dashboard");
            }
            ViewBag.Error = "اسم المستخدم أو كلمة المرور غير صحيحة";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}