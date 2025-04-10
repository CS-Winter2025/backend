using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace CourseProject.Areas.Housing.Controllers
{
    [Area("Housing")]
    public class AssetsController : Controller
    {
        private readonly DatabaseContext _context;

        public AssetsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Assets
        [Authorize(Roles = nameof(UserRole.RESIDENT) + "," + nameof(UserRole.HOUSING_MANAGER) + "," + nameof(UserRole.ADMIN))]
        public async Task<IActionResult> Index()
        {
            var assets = await _context.Assets.ToListAsync();
            var now = DateTime.Now;

            var assignments = await _context.ResidentAssets
    .Include(a => a.Resident)
    .Where(ra => ra.FromDate <= now && ra.ToDate >= now)
    .ToListAsync();


            var assetStatusList = assets.Select(asset =>
            {
                var activeAssignment = assignments.FirstOrDefault(a => a.AssetID == asset.AssetID);
                return new AssetStatusViewModel
                {
                    Asset = asset,
                    Status = activeAssignment != null ? "In use" : "Available",
                    AssignedResident = activeAssignment?.Resident
                };
            }).ToList();


            var requests = await _context.ResidentAssetRequests
                .Include(r => r.Resident)
                .Include(r => r.Asset)
                .ToListAsync();

            ViewBag.PendingRequests = requests;

            ViewBag.AvailableAssets = assetStatusList.Where(a => a.Status == "Available").ToList();
            ViewBag.InUseAssets = assetStatusList.Where(a => a.Status == "In use").ToList();

            return View();
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> ApproveRequest(int requestId)
        {
            var request = await _context.ResidentAssetRequests
                .Include(r => r.Resident)
                .Include(r => r.Asset)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null) return NotFound();

            var assignment = new ResidentAsset
            {
                ResidentId = request.ResidentId,
                AssetID = request.AssetID,
                FromDate = request.FromDate,
                ToDate = request.ToDate
            };

            _context.ResidentAssets.Add(assignment);
            _context.ResidentAssetRequests.Remove(request); // remove request
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        [HttpPost]
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> DeclineRequest(int requestId)
        {
            var request = await _context.ResidentAssetRequests.FindAsync(requestId);
            if (request == null) return NotFound();

            _context.ResidentAssetRequests.Remove(request);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: Assets/Assign/5
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> Assign(int id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset == null) return NotFound();

            var residents = await _context.Residents.ToListAsync();
            ViewBag.AssetId = id;
            return View(residents);
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> Assign(int assetId, List<ResidentAssignmentViewModel> selectedResidents)
        {
            foreach (var item in selectedResidents)
            {
                if (item.IsSelected)
                {
                    var assignment = new ResidentAsset
                    {
                        AssetID = assetId,
                        ResidentId = item.ResidentId,
                        FromDate = item.FromDate,
                        ToDate = item.ToDate
                    };
                    _context.ResidentAssets.Add(assignment);

                    //Update asset status
                    var asset = await _context.Assets.FindAsync(assetId);

                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Assets/Details/5
        [Authorize(Roles = nameof(UserRole.RESIDENT) + "," + nameof(UserRole.HOUSING_MANAGER) + "," + nameof(UserRole.ADMIN))]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asset = await _context.Assets
                .FirstOrDefaultAsync(m => m.AssetID == id);
            if (asset == null)
            {
                return NotFound();
            }

            return View(asset);
        }

        // GET: Assets/Create
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Assets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> Create([Bind("AssetID,Type,DetailsJson,Price")] Asset asset)
        {
            if (ModelState.IsValid)
            {
                asset.Status = "Available"; //default status
                _context.Add(asset);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(asset);
        }

        // GET: Assets/Edit/5
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var asset = await _context.Assets.FindAsync(id);
            if (asset == null) return NotFound();

            return View(asset);
        }

        // POST: Assets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> Edit(int id, [Bind("AssetID,Type,DetailsJson,Price")] Asset asset)
        {
            if (id != asset.AssetID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(asset);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Assets.Any(a => a.AssetID == asset.AssetID))
                        return NotFound();
                    throw;
                }
            }

            return View(asset);
        }


        // GET: Assets/Delete/5
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asset = await _context.Assets
                .FirstOrDefaultAsync(m => m.AssetID == id);
            if (asset == null)
            {
                return NotFound();
            }

            return View(asset);
        }

        // POST: Assets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset != null)
            {
                _context.Assets.Remove(asset);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssetExists(int id)
        {
            return _context.Assets.Any(e => e.AssetID == id);
        }

        // GET: Assets/Available
        [Authorize(Roles = nameof(UserRole.RESIDENT) + "," + nameof(UserRole.HOUSING_MANAGER) + "," + nameof(UserRole.ADMIN))]
        public async Task<IActionResult> Available()
        {
            var availableAssets = await _context.Assets
                .Where(a => a.Status == "Available")
                .ToListAsync();
            return View(availableAssets);
        }

        // GET: Assets/Assigned
        [Authorize(Roles = nameof(UserRole.RESIDENT) + "," + nameof(UserRole.HOUSING_MANAGER) + "," + nameof(UserRole.ADMIN))]
        public async Task<IActionResult> Assigned()
        {
            var assignedAssets = await _context.Assets
                .Where(a => a.Status == "In use")
                .ToListAsync();
            return View(assignedAssets);
        }

        // POST: Assets/CancelRequest/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> CancelRequest(int id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset == null)
            {
                return NotFound();
            }

            // Logic to cancel asset request
            // E.g., asset.AssignedTo = null;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Assigned));
        }
        
        // GET: Assets/PastRenters/5
        public async Task<IActionResult> PastRenters(int id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset == null)
            {
                return NotFound();
            }

            var pastRenters = await _context.ResidentAssets
                .Where(ra => ra.AssetID == id && ra.ToDate < DateTime.Now)
                .Include(ra => ra.Resident)
                .ToListAsync();

            ViewBag.Asset = asset;
            return View(pastRenters);
        }


        // POST: Assets/MarkVacant/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkVacant(int id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset == null) return NotFound();

            // Optionally: close the current assignment
            var currentAssignment = await _context.ResidentAssets
                .Where(ra => ra.AssetID == id && ra.FromDate <= DateTime.Now && ra.ToDate >= DateTime.Now)
                .OrderByDescending(ra => ra.ToDate)
                .FirstOrDefaultAsync();

            if (currentAssignment != null)
            {
                currentAssignment.ToDate = DateTime.Now; // renter vacates now
            }

            asset.Status = "Available";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> MonthlyInvoices(int? month, int? year)
        {
            var targetMonth = month ?? DateTime.Now.Month;
            var targetYear = year ?? DateTime.Now.Year;

            var startDate = new DateTime(targetYear, targetMonth, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1); // Last day of the month

            var assignments = await _context.ResidentAssets
                .Include(ra => ra.Resident)
                .Include(ra => ra.Asset)
                .Where(ra => ra.ToDate >= startDate && ra.FromDate <= endDate)
            .ToListAsync();

            ViewBag.Month = targetMonth;
            ViewBag.Year = targetYear;
            return View(MonthlyInvoices);


        }
    }
}
