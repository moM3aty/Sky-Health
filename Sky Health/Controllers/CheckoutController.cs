using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Sky_Health.Data;
using Sky_Health.Helpers;
using Sky_Health.Hubs;
using Sky_Health.Models;
using Sky_Health.Services;
using Sky_Health.ViewModels;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sky_Health.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPaymobService _paymobService;
        private readonly IHubContext<NotificationHub> _hubContext;
        public CheckoutController(ApplicationDbContext context, IPaymobService paymobService, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _paymobService = paymobService;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var cart = SessionHelper.GetObjectFromJson<CartViewModel>(HttpContext.Session, "Cart");
            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new CheckoutViewModel { Cart = cart };

            await RehydrateCheckoutViewData();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {

            var cart = SessionHelper.GetObjectFromJson<CartViewModel>(HttpContext.Session, "Cart");
            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("Index", "Home");
            }
            model.Cart = cart;
            ModelState.Remove("Cart");

            if (ModelState.IsValid)
            {
                var shippingZone = await _context.ShippingZones.FindAsync(model.ShippingZoneId);
                if (shippingZone == null)
                {
                    ModelState.AddModelError("", "منطقة الشحن غير صالحة.");
                    await RehydrateCheckoutViewData();
                    return View("Index", model); 
                }

                var order = new Order
                {
                    CustomerName = model.CustomerName,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    ShippingCost = shippingZone.Price,
                    TotalAmount = model.Cart.Total + shippingZone.Price,
                    ShippingZoneName = shippingZone.Name,
                    PaymentMethod = model.PaymentMethod,
                    OrderDate = DateTime.Now,
                    OrderStatus = "جديد"

                };

                foreach (var item in model.Cart.Items)
                {
                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        UnitPrice = item.Price
                    });
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                var notificationMessage = $"طلب جديد برقم #{order.Id} من {order.CustomerName}";
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", notificationMessage, order.Id);
                HttpContext.Session.Remove("Cart");

                if (model.PaymentMethod == "Visa")
                {
                    try
                    {
                        var paymentUrl = await _paymobService.CreatePaymentUrl(order);
                        if (!string.IsNullOrEmpty(paymentUrl))
                        {
                            return Redirect(paymentUrl);
                        }
                        else
                        {
                            TempData["Error"] = "حدث خطأ أثناء محاولة الدفع. يرجى المحاولة مرة أخرى.";
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    catch (Exception)
                    {
                        TempData["Error"] = "حدث خطأ غير متوقع. يرجى التواصل مع الدعم.";
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    switch (model.PaymentMethod)
                    {
                        case "VodafoneCash":
                            return RedirectToAction("VodafoneCash", new { orderId = order.Id });
                        case "InstaPay":
                            return RedirectToAction("InstaPay", new { orderId = order.Id });
                        case "WePay":
                            return RedirectToAction("WePay", new { orderId = order.Id });
                            
                        default: 
                            return RedirectToAction("Success", new { orderId = order.Id });
                    }
                }
            }

            await RehydrateCheckoutViewData();
            return View("Index", model);
        }

        public async Task<IActionResult> VodafoneCash(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return NotFound();
            return View(order);
        }

        public async Task<IActionResult> InstaPay(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return NotFound();
            return View(order);
        }
        public async Task<IActionResult> WePay(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return NotFound();
            return View(order);
        }
        public async Task<IActionResult> Success(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null && order.OrderStatus == "في انتظار الدفع")
            {
                order.OrderStatus = "جديد";
                await _context.SaveChangesAsync();
            }
            ViewBag.OrderId = orderId;
            return View();
        }

        private async Task RehydrateCheckoutViewData()
        {
            var zones = await _context.ShippingZones.ToListAsync();
            var formattedZones = zones.Select(z => new { Id = z.Id, DisplayText = $"{z.Name} ({z.Price:N2} جنيه)" }).ToList();
            ViewData["ShippingZones"] = new SelectList(formattedZones, "Id", "DisplayText");
            ViewData["ShippingZonesJson"] = JsonSerializer.Serialize(zones.Select(z => new { id = z.Id, price = z.Price }));
        }
    }
}