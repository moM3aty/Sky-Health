using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sky_Health.Data;
using Sky_Health.Models;
using Sky_Health.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Sky_Health.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ShippingZonesAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShippingZonesAdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string searchTerm)
        {
            var query = _context.ShippingZones.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(z => z.Name.Contains(searchTerm));
            }

            var viewModel = new ShippingZonesAdminViewModel
            {
                ShippingZones = await query.ToListAsync(),
                SearchTerm = searchTerm
            };

            return View(viewModel);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Price")] ShippingZone shippingZone)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shippingZone);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(shippingZone);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var shippingZone = await _context.ShippingZones.FindAsync(id);
            if (shippingZone == null) return NotFound();
            return View(shippingZone);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price")] ShippingZone shippingZone)
        {
            if (id != shippingZone.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shippingZone);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ShippingZones.Any(e => e.Id == shippingZone.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(shippingZone);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shippingZone = await _context.ShippingZones
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shippingZone == null)
            {
                return NotFound();
            }

            return View(shippingZone);
        }
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shippingZone = await _context.ShippingZones.FindAsync(id);
            if (shippingZone != null)
            {
                _context.ShippingZones.Remove(shippingZone);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}