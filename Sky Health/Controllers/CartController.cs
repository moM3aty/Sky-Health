using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sky_Health.Data;
using Sky_Health.Helpers;
using Sky_Health.Models;
using Sky_Health.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Sky_Health.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CartController(ApplicationDbContext context) { _context = context; }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return Json(new { success = false, message = "المنتج غير موجود." });

            var cart = SessionHelper.GetObjectFromJson<CartViewModel>(HttpContext.Session, "Cart") ?? new CartViewModel();
            var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (cartItem != null) { cartItem.Quantity += quantity; }
            else
            {
                cart.Items.Add(new CartItemViewModel
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    ImageUrl = product.ImageUrl ?? "/images/product1.jpg"
                });
            }
            SessionHelper.SetObjectAsJson(HttpContext.Session, "Cart", cart);
            return PartialView("_CartPartial", cart);
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = SessionHelper.GetObjectFromJson<CartViewModel>(HttpContext.Session, "Cart") ?? new CartViewModel();
            var itemToRemove = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (itemToRemove != null) { cart.Items.Remove(itemToRemove); }
            SessionHelper.SetObjectAsJson(HttpContext.Session, "Cart", cart);
            return PartialView("_CartPartial", cart);
        }

        public IActionResult GetCartPartial()
        {
            var cart = SessionHelper.GetObjectFromJson<CartViewModel>(HttpContext.Session, "Cart") ?? new CartViewModel();
            return PartialView("_CartPartial", cart);
        }
    }
}