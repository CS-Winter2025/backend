using System.Security.Claims;
using CourseProject.Common;
using CourseProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Areas.Housing.Controllers
{
    [Area("Housing")]
    public class ResidentsController : Controller
    {
        private readonly DatabaseContext _context;

        public ResidentsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Residents
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;

            var currentResidents = await _context.ResidentAssets
                .Include(ra => ra.Resident)
                .Where(ra => ra.FromDate <= now && ra.ToDate >= now)
                .Select(ra => ra.Resident)
                .Distinct()
                .ToListAsync();

            var pastResidents = await _context.ResidentAssets
                .Include(ra => ra.Resident)
                .Where(ra => ra.ToDate < now)
                .Select(ra => ra.Resident)
                .Distinct()
                .ToListAsync();

            var allAssignedResidentIds = await _context.ResidentAssets
                .Select(ra => ra.ResidentId)
                .Distinct()
                .ToListAsync();

            var assignedNowIds = await _context.ResidentAssets
                .Where(ra => ra.FromDate <= now && ra.ToDate >= now)
                .Select(ra => ra.ResidentId)
                .Distinct()
                .ToListAsync();

            var unassignedResidents = await _context.Residents
                .Where(r => !assignedNowIds.Contains(r.ResidentId))
                .ToListAsync();

            var allResidents = currentResidents.Concat(pastResidents).Concat(unassignedResidents).DistinctBy(r => r.ResidentId).ToList();

            var parsedDetails = allResidents.ToDictionary(
                r => r.ResidentId,
                r => Util.ParseJson(r.DetailsJson ?? string.Empty) ?? new Dictionary<string, string>()
            );

            ViewBag.CurrentResidents = currentResidents;
            ViewBag.PastResidents = pastResidents;
            ViewBag.UnassignedResidents = unassignedResidents;
            ViewBag.ParsedDetails = parsedDetails;

            return View();
        }

        [Authorize(Roles = nameof(UserRole.RESIDENT) + "," + nameof(UserRole.ADMIN))]
        public async Task<IActionResult> Me()
        {
            string? stringId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (stringId == null) return RedirectToAction("Forbidden", "Error");

            int id = Int32.Parse(stringId);
            User? user = await _context.Users.Include(u => u.Resident)
                                .ThenInclude(r => r.Services)
                                .FirstOrDefaultAsync(u => u.Id == id);

            ViewBag.Details = user.Resident.DetailsJson != null
                ? Util.ParseJson(user.Resident.DetailsJson)
                : new Dictionary<string, string>();

            if (user == null) return RedirectToAction("NotFound", "Error");
            return View(user);
        }

        // GET: Residents/Details/5
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resident = await _context.Residents
                .Include(r => r.Services)
                .FirstOrDefaultAsync(m => m.ResidentId == id);
            if (resident == null)
            {
                return NotFound();
            }

            ViewBag.Details = resident.DetailsJson != null
                ? Util.ParseJson(resident.DetailsJson)
                : new Dictionary<string, string>();

            return View(resident);
        }

        // GET: Residents/Create
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public IActionResult Create()
        {
            var resident = new Resident
            {
                Name = new FullName(),
                Address = new FullAddress()
            };
            return View(resident);
        }

        // POST: Residents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> Create([Bind("ResidentId,ServiceSubscriptionIds,Name,Address,DetailsJson,ProfilePicture")] Resident resident, IFormFile ProfilePicture)
        {
            if (ModelState.IsValid)
            {
                if (ProfilePicture != null && ProfilePicture.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await ProfilePicture.CopyToAsync(memoryStream);
                        resident.ProfilePicture = memoryStream.ToArray();  // Convert the file to byte array
                    }
                }

                _context.Add(resident);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(resident);
        }

        // GET: Residents/Edit/5
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resident = await _context.Residents.FindAsync(id);
            if (resident == null)
            {
                return NotFound();
            }

            ViewBag.Details = resident.DetailsJson != null
                ? Util.ParseJson(resident.DetailsJson)
                : new Dictionary<string, string>();

            return View(resident);
        }

        // POST: Residents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> Edit(int id, [Bind("ResidentId,ServiceSubscriptionIds,Name,Address,DetailsJson")] Resident resident, IFormFile ProfilePicture)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    var existingResident = await _context.Residents.FindAsync(id);

                    if (existingResident == null)
                    {
                        return NotFound();
                    }

                    if (ProfilePicture != null && ProfilePicture.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await ProfilePicture.CopyToAsync(memoryStream);
                            existingResident.ProfilePicture = memoryStream.ToArray();
                        }
                    }
                    existingResident.ResidentId = resident.ResidentId;
                    existingResident.ServiceSubscriptionIds = resident.ServiceSubscriptionIds;
                    existingResident.Name = resident.Name;
                    existingResident.Address = resident.Address;
                    existingResident.DetailsJson = resident.DetailsJson;
                    _context.Update(existingResident);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResidentExists(resident.ResidentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Details = Util.ParseJson(resident.DetailsJson ?? "");
            return View(resident);
        }

        // GET: Residents/Delete/5
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resident = await _context.Residents
                .FirstOrDefaultAsync(m => m.ResidentId == id);
            if (resident == null)
            {
                return NotFound();
            }

            ViewBag.Details = resident.DetailsJson != null
                ? Util.ParseJson(resident.DetailsJson)
                : new Dictionary<string, string>();

            return View(resident);
        }

        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> AssignedAssets(int id)
        {
            var resident = await _context.Residents.FindAsync(id);
            if (resident == null)
            {
                return NotFound();
            }

            var assignments = await _context.ResidentAssets
                                    .Include(ra => ra.Asset)
                                    .Where(ra => ra.ResidentId == id)
                                    .ToListAsync();

            ViewBag.ResidentName = resident.Name;
            return View(assignments);

        }


        // POST: Residents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var resident = await _context.Residents.FindAsync(id);
            if (resident != null)
            {
                _context.Residents.Remove(resident);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ResidentExists(int id)
        {
            return _context.Residents.Any(e => e.ResidentId == id);
        }

        // GET: Residents/Current
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> Current()
        {
            var currentRenters = await _context.Residents
                .Where(r => r.IsCurrentlyLiving == true)
                .ToListAsync();
            return View(currentRenters);
        }

        // GET: Residents/Past
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> Past()
        {
            var pastRenters = await _context.Residents
                .Where(r => r.IsCurrentlyLiving == false)
                .ToListAsync();
            return View(pastRenters);
        }

        // POST: Residents/UpdateOccupants/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> UpdateOccupants(int id, [FromForm] string newOccupantsJson)
        {
            var resident = await _context.Residents.FindAsync(id);
            if (resident == null)
            {
                return NotFound();
            }

            // Logic to update occupants (deserialize JSON and save)
            resident.DetailsJson = newOccupantsJson;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Residents/MarkVacant/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> MarkVacant(int id)
        {
            var resident = await _context.Residents.FindAsync(id);
            if (resident == null)
            {
                return NotFound();
            }

            resident.IsCurrentlyLiving = false;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Current));
        }

    }
}
