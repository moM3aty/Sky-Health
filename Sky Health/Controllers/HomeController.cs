using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sky_Health.Data;
using Sky_Health.Models;
using Sky_Health.ViewModels;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Sky_Health.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context) { _context = context; }

        public async Task<IActionResult> Index()
        {
            var allRegularProducts = await _context.Products
                                                   .Include(p => p.Category)
                                                   .Where(p => p.Type == ProductType.Regular)
                                                   .OrderByDescending(p => p.Id) 
                                                   .ToListAsync();

            var productsGrouped = allRegularProducts
                                    .GroupBy(p => p.Category)
                                    .ToDictionary(g => g.Key, g => g.Take(4).ToList());

            var viewModel = new HomeViewModel
            {
                FeaturedProducts = allRegularProducts.Where(p => p.IsFeatured).Take(6),
                ProductsByCategory = productsGrouped,
                LatestBlogs = await _context.Blogs.OrderByDescending(b => b.CreatedDate).Take(3).ToListAsync()
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(m => m.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        public async Task<IActionResult> Blogs()
        {
            var blogs = await _context.Blogs.OrderByDescending(b => b.CreatedDate).ToListAsync();
            return View(blogs);
        }

        public async Task<IActionResult> BlogDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }

       
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}