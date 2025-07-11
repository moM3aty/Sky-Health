using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sky_Health.Data;
using Sky_Health.Models;
using Sky_Health.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Sky_Health.Controllers
{
    public class PharmacyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string PharmacySessionKey = "_PharmacyAccess";

        public PharmacyController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString(PharmacySessionKey) == "Granted")
            {
                return RedirectToAction("Products");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyCode(string accessCode)
        {
            if (string.IsNullOrEmpty(accessCode))
            {
                TempData["Error"] = "الرجاء إدخال كود الدخول.";
                return RedirectToAction("Index");
            }

            var isValid = await _context.PharmacyAccessCodes.AnyAsync(c => c.Code == accessCode && c.IsActive);

            if (isValid)
            {
                HttpContext.Session.SetString(PharmacySessionKey, "Granted");
                return RedirectToAction("Products");
            }

            TempData["Error"] = "كود الدخول غير صحيح أو غير فعال.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Products()
        {
            if (HttpContext.Session.GetString(PharmacySessionKey) != "Granted")
            {
                return RedirectToAction("Index");
            }

            var pharmacyProducts = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Type == ProductType.Pharmacy)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            var productsGrouped = pharmacyProducts
                                    .GroupBy(p => p.Category)
                                    .ToDictionary(g => g.Key, g => g.Take(4).ToList());

            return View(productsGrouped);
        }

        public async Task<IActionResult> AllProducts(int? categoryId, string searchTerm)
        {
            if (HttpContext.Session.GetString(PharmacySessionKey) != "Granted")
            {
                return RedirectToAction("Index");
            }

            var productsQuery = _context.Products
                                        .Include(p => p.Category)
                                        .Where(p => p.Type == ProductType.Pharmacy)
                                        .AsQueryable();

            if (categoryId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(searchTerm));
            }

            var viewModel = new PharmacyProductsViewModel
            {
                Products = await productsQuery.ToListAsync(),
                AllCategories = await _context.Categories.Where(c => c.Products.Any(p => p.Type == ProductType.Pharmacy)).ToListAsync(),
                CurrentCategoryId = categoryId,
                SearchTerm = searchTerm
            };

            return View(viewModel);
        }
    }
}