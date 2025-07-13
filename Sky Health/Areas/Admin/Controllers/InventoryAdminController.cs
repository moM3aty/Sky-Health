using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sky_Health.Data;
using Sky_Health.Models;
using Sky_Health.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Sky_Health.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class InventoryAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InventoryAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchTerm, int? categoryId, string stockStatus)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrEmpty(stockStatus))
            {
                switch (stockStatus)
                {
                    case "InStock":
                        query = query.Where(p => p.StockQuantity > 10);
                        break;
                    case "LowStock":
                        query = query.Where(p => p.StockQuantity > 0 && p.StockQuantity <= 10);
                        break;
                    case "OutOfStock":
                        query = query.Where(p => p.StockQuantity == 0);
                        break;
                }
            }

            var viewModel = new InventoryViewModel
            {
                Products = await query.ToListAsync(),
                Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", categoryId),
                SearchTerm = searchTerm,
                CurrentCategoryId = categoryId,
                CurrentStockStatus = stockStatus
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStock(int id, int quantity)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            if (quantity < 0)
            {
                return BadRequest("الكمية لا يمكن أن تكون سالبة.");
            }

            product.StockQuantity = quantity;
            await _context.SaveChangesAsync();

            return Ok(new { newQuantity = product.StockQuantity });
        }
    }
}
