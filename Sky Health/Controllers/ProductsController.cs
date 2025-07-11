using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sky_Health.Data;
using Sky_Health.Models;
using Sky_Health.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Sky_Health.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? categoryId, string searchTerm)
        {
            var productsQuery = _context.Products
                                        .Include(p => p.Category)
                                        .Where(p => p.Type == ProductType.Regular)
                                        .AsQueryable();

            if (categoryId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(searchTerm));
            }

            var viewModel = new ProductsViewModel
            {
                Products = await productsQuery.ToListAsync(),
                AllCategories = await _context.Categories.ToListAsync(),
                CurrentCategoryId = categoryId,
                SearchTerm = searchTerm
            };

            return View(viewModel);
        }
    }
}