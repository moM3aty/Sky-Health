using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sky_Health.Data;
using Sky_Health.Models;
using Sky_Health.ViewModels;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sky_Health.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductsAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsAdminController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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

            var viewModel = new ProductsAdminViewModel
            {
                Products = await query.OrderByDescending(p => p.Id).ToListAsync(),
                Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", categoryId),
                SearchTerm = searchTerm,
                CurrentCategoryId = categoryId,
                CurrentStockStatus = stockStatus
            };

            return View(viewModel);
        }

        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,OldPrice,StockQuantity,UnitsPerBox,Type,IsFeatured,CategoryId")] Product product, IFormFile? imageFile)
        {
            if (product.Type != ProductType.Pharmacy)
            {
                product.UnitsPerBox = null;
            }

            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    product.ImageUrl = await UploadImage(imageFile);
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,OldPrice,StockQuantity,UnitsPerBox,Type,IsFeatured,CategoryId,ImageUrl")] Product product, IFormFile? imageFile)
        {
            if (id != product.Id) return NotFound();

            if (product.Type != ProductType.Pharmacy)
            {
                product.UnitsPerBox = null;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null)
                    {
                        if (!string.IsNullOrEmpty(product.ImageUrl))
                        {
                            DeleteImage(product.ImageUrl);
                        }
                        product.ImageUrl = await UploadImage(imageFile);
                    }
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(m => m.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    DeleteImage(product.ImageUrl);
                }
                _context.Products.Remove(product);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        private async Task<string> UploadImage(IFormFile imageFile)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            string productPath = Path.Combine(wwwRootPath, "images/products");
            if (!Directory.Exists(productPath))
            {
                Directory.CreateDirectory(productPath);
            }
            using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            return "/images/products/" + fileName;
        }

        private void DeleteImage(string imageUrl)
        {
            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl.TrimStart('/'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(string categoryName)
        {
            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                var category = new Category { Name = categoryName };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return Json(new { success = true, id = category.Id, name = category.Name });
            }
            return Json(new { success = false });
        }
    }
}
