using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sky_Health.Data;
using Sky_Health.Models;

namespace Sky_Health.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class PharmacyCodesAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PharmacyCodesAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchTerm)
        {
            var query = _context.PharmacyAccessCodes.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(b => b.Code.Contains(searchTerm));
            }

            ViewData["CurrentFilter"] = searchTerm;

            return View(await query.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create()
        {
            var newCode = new PharmacyAccessCode
            {
                Code = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper(),
                IsActive = true
            };

            _context.PharmacyAccessCodes.Add(newCode);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var code = await _context.PharmacyAccessCodes.FindAsync(id);
            if (code != null)
            {
                code.IsActive = !code.IsActive;
                await _context.SaveChangesAsync();
                return Ok();
            }
            return NotFound();
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pharmacyAccessCode = await _context.PharmacyAccessCodes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pharmacyAccessCode == null)
            {
                return NotFound();
            }

            return View(pharmacyAccessCode);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var code = await _context.PharmacyAccessCodes.FindAsync(id);
            if (code != null)
            {
                _context.PharmacyAccessCodes.Remove(code);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
