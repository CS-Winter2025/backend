using CourseProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Areas.Housing.Controllers
{
    [Area("Housing")]
    public class ResidentAssetsController : Controller
    {
        private readonly DatabaseContext _context;

        public ResidentAssetsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Housing/ResidentAssets
        [Authorize(Roles = nameof(UserRole.RESIDENT))]
        public async Task<IActionResult> Index()
        {
            var availableAssets = await _context.Assets
                .Where(a => !_context.ResidentAssets
                    .Any(ra => ra.AssetID == a.AssetID &&
                               ra.FromDate <= DateTime.Now &&
                               ra.ToDate >= DateTime.Now))
                .ToListAsync();

            return View(availableAssets);
        }

        // GET: Housing/ResidentAssets/Details/5
        [Authorize(Roles = nameof(UserRole.RESIDENT))]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var asset = await _context.Assets.FirstOrDefaultAsync(a => a.AssetID == id);
            if (asset == null) return NotFound();

            return View(asset);
        }

        // POST: Housing/ResidentAssets/Request/5
        [HttpPost]
        [Authorize(Roles = nameof(UserRole.RESIDENT))]
        public async Task<IActionResult> Request(int id)
        {
            var userName = User.Identity.Name;
            var resident = await _context.Residents.FirstOrDefaultAsync(r => r.Name == userName);

            if (resident == null)
                return NotFound("Resident not found");

            var request = new ResidentAssetRequest
            {
                ResidentId = resident.ResidentId,
                AssetID = id
            };

            _context.ResidentAssetRequests.Add(request);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
