using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sky_Health.Data;
using Sky_Health.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sky_Health.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public PaymentController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Callback([FromQuery] PaymobCallbackQuery query)
        {
            var hmacSecret = _configuration["Paymob:HmacSecret"];
            if (!IsValidCallback(query, hmacSecret)) return RedirectToAction("PaymentFailed");
            if (!int.TryParse(query.merchant_order_id, out int orderId)) return BadRequest("Invalid Order ID.");

            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return NotFound();

            if (query.success == "true" && query.pending == "false")
            {
                order.OrderStatus = "مدفوع";
                await _context.SaveChangesAsync();
                return RedirectToAction("PaymentSuccess", new { orderId = order.Id });
            }
            else
            {
                order.OrderStatus = "فشل الدفع";
                await _context.SaveChangesAsync();
                return RedirectToAction("PaymentFailed", new { orderId = order.Id });
            }
        }

        private bool IsValidCallback(PaymobCallbackQuery query, string hmacSecret)
        {
            var parameters = new SortedDictionary<string, string>
            {
                { "amount_cents", query.amount_cents }, { "created_at", query.created_at }, { "currency", query.currency },
                { "error_occured", query.error_occured }, { "has_parent_transaction", query.has_parent_transaction },
                { "id", query.id }, { "integration_id", query.integration_id }, { "is_3d_secure", query.is_3d_secure },
                { "is_auth", query.is_auth }, { "is_capture", query.is_capture }, { "is_refunded", query.is_refunded },
                { "is_standalone_payment", query.is_standalone_payment }, { "is_voided", query.is_voided },
                { "merchant_order_id", query.merchant_order_id }, { "order", query.order }, { "owner", query.owner },
                { "pending", query.pending }, { "source_data_pan", query.source_data_pan },
                { "source_data_sub_type", query.source_data_sub_type }, { "source_data_type", query.source_data_type },
                { "success", query.success }
            };
            var concatenatedString = string.Concat(parameters.Where(p => p.Value != null).Select(p => p.Value));
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(hmacSecret));
            var computedHash = BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(concatenatedString))).Replace("-", "").ToLower();
            return computedHash == query.hmac;
        }

        public IActionResult PaymentSuccess(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }

        public IActionResult PaymentFailed(int? orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }
    }

    public class PaymobCallbackQuery
    {
        public string amount_cents { get; set; }
        public string created_at { get; set; }
        public string currency { get; set; }
        public string error_occured { get; set; }
        public string has_parent_transaction { get; set; }
        public string id { get; set; }
        public string integration_id { get; set; }
        public string is_3d_secure { get; set; }
        public string is_auth { get; set; }
        public string is_capture { get; set; }
        public string is_refunded { get; set; }
        public string is_standalone_payment { get; set; }
        public string is_voided { get; set; }
        public string merchant_order_id { get; set; }
        public string order { get; set; }
        public string owner { get; set; }
        public string pending { get; set; }
        public string source_data_pan { get; set; }
        public string source_data_sub_type { get; set; }
        public string source_data_type { get; set; }
        public string success { get; set; }
        public string hmac { get; set; }
    }
}