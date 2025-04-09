using CourseProject.Models;
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
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var asset = await _context.Assets.FirstOrDefaultAsync(a => a.AssetID == id);
            if (asset == null) return NotFound();

            return View(asset);
        }

        // POST: Housing/ResidentAssets/Request
        [HttpPost]
        public async Task<IActionResult> Request(int AssetID, DateTime FromDate, DateTime ToDate)
        {
            var sessionUserId = HttpContext.Session.GetInt32("UserId");

            if (sessionUserId == null)
            {
                TempData["Error"] = "You must be logged in to request assets.";
                return RedirectToAction("Index", "Home");
            }

            var user = await _context.Users
                .Include(u => u.Resident)
                .FirstOrDefaultAsync(u => u.Id == sessionUserId);

            if (user == null || user.Resident == null)
            {
                TempData["Error"] = "Only residents can make asset requests.";
                return RedirectToAction("Index", "Home");
            }

            var request = new ResidentAssetRequest
            {
                ResidentId = user.Resident.ResidentId,
                AssetID = AssetID,
                FromDate = FromDate,
                ToDate = ToDate,
                RequestDate = DateTime.Now
            };

            _context.ResidentAssetRequests.Add(request);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Your request has been submitted!";
            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> PredictedInvoice()
        {
            var sessionUserId = HttpContext.Session.GetInt32("UserId");

            if (sessionUserId == null)
            {
                TempData["Error"] = "You must be logged in.";
                return RedirectToAction("Index", "Home");
            }

            var user = await _context.Users
                .Include(u => u.Resident)
                .FirstOrDefaultAsync(u => u.Id == sessionUserId);

            if (user == null || user.Resident == null)
            {
                TempData["Error"] = "Only residents can view predicted invoices.";
                return RedirectToAction("Index", "Home");
            }

            var now = DateTime.Now;
            var futureAssignments = await _context.ResidentAssets
                .Include(ra => ra.Asset)
                .Where(ra => ra.ResidentId == user.Resident.ResidentId && ra.ToDate >= now)
                .ToListAsync();

            var invoiceItems = futureAssignments.Select(a => new
            {
                a.Asset.Type,
                a.Asset.Price,
                a.FromDate,
                a.ToDate,
                Days = (a.ToDate - a.FromDate).Days + 1,
                Total = a.Asset.Price * ((a.ToDate - a.FromDate).Days + 1)
            }).ToList();

            ViewBag.InvoiceItems = invoiceItems;
            ViewBag.TotalCost = invoiceItems.Sum(i => i.Total);

            return View();
        }

        public async Task<IActionResult> MyAssets()
        {
            var sessionUserId = HttpContext.Session.GetInt32("UserId");

            if (sessionUserId == null)
            {
                TempData["Error"] = "You must be logged in.";
                return RedirectToAction("Index", "Home");
            }

            var user = await _context.Users
                .Include(u => u.Resident)
                .FirstOrDefaultAsync(u => u.Id == sessionUserId);

            if (user == null || user.Resident == null)
            {
                TempData["Error"] = "Only residents can view their assets.";
                return RedirectToAction("Index", "Home");
            }

            var myAssignments = await _context.ResidentAssets
                .Include(ra => ra.Asset)
                .Where(ra => ra.ResidentId == user.Resident.ResidentId)
                .ToListAsync();

            return View(myAssignments);
        }


        // GET: EditOccupants
        public async Task<IActionResult> EditOccupants(int id)
        {
            var assignment = await _context.ResidentAssets
                .Include(ra => ra.Asset)
                .FirstOrDefaultAsync(ra => ra.Id == id);

            if (assignment == null)
            {
                return NotFound();
            }

            ViewBag.AssetInfo = assignment.Asset.Type;
            ViewBag.AssignmentId = assignment.Id;
            ViewBag.FromDate = assignment.FromDate;
            ViewBag.ToDate = assignment.ToDate;

            return View(model: assignment.Asset.DetailsJson ?? "");
        }

        // POST: EditOccupants
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOccupants(int id, string newOccupantsJson)
        {
            var assignment = await _context.ResidentAssets
                .Include(ra => ra.Asset)
                .FirstOrDefaultAsync(ra => ra.Id == id);

            if (assignment == null) return NotFound();

            assignment.Asset.DetailsJson = newOccupantsJson;
            _context.Update(assignment.Asset);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Occupants updated successfully.";
            return RedirectToAction("MyAssets");
        }


        public async Task<IActionResult> MyInvoices(int? month, int? year)
        {
            var sessionUserId = HttpContext.Session.GetInt32("UserId");

            if (sessionUserId == null)
            {
                TempData["Error"] = "You must be logged in.";
                return RedirectToAction("Index", "Home");
            }

            var user = await _context.Users
                .Include(u => u.Resident)
                .FirstOrDefaultAsync(u => u.Id == sessionUserId);

            if (user?.Resident == null)
            {
                TempData["Error"] = "Only residents can view invoices.";
                return RedirectToAction("Index", "Home");
            }

            var targetMonth = month ?? DateTime.Now.Month;
            var targetYear = year ?? DateTime.Now.Year;
            var startDate = new DateTime(targetYear, targetMonth, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var assignments = await _context.ResidentAssets
                .Include(ra => ra.Asset)
                .Where(ra => ra.ResidentId == user.Resident.ResidentId &&
                             ra.ToDate >= startDate && ra.FromDate <= endDate)
                .ToListAsync();

            var items = assignments.Select(ra =>
            {
                var from = ra.FromDate < startDate ? startDate : ra.FromDate;
                var to = ra.ToDate > endDate ? endDate : ra.ToDate;
                var days = (to - from).Days + 1;

                return new InvoiceItemViewModel
                {
                    AssetType = ra.Asset.Type,
                    Days = days,
                    DailyRate = ra.Asset.Price,
                    Total = days * ra.Asset.Price
                };
            }).ToList();

            var invoice = new ResidentInvoiceViewModel
            {
                Resident = user.Resident,
                Items = items,
                TotalDue = items.Sum(i => i.Total)
            };

            ViewBag.Month = targetMonth;
            ViewBag.Year = targetYear;

            return View(invoice);
        }

    }
}
