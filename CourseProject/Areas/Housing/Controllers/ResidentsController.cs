using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

            var unassignedResidents = await _context.Residents
                .Where(r => !allAssignedResidentIds.Contains(r.ResidentId))
                .ToListAsync();

            ViewBag.CurrentResidents = currentResidents;
            ViewBag.PastResidents = pastResidents;
            ViewBag.UnassignedResidents = unassignedResidents;

            return View();
        }



        // GET: Residents/Details/5
        public async Task<IActionResult> Details(int? id)
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

            return View(resident);
        }

        // GET: Residents/Create
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
        public async Task<IActionResult> Create([Bind("ResidentId,ServiceSubscriptionIds,Name,Address,DetailsJson")] Resident resident)
        {
            if (ModelState.IsValid)
            {
                _context.Add(resident);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(resident);
        }

        // GET: Residents/Edit/5
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
            return View(resident);
        }

        // POST: Residents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ResidentId,ServiceSubscriptionIds,Name,Address,DetailsJson")] Resident resident)
        {
            //if (id != resident.ResidentId)
            //{
            //    return NotFound();
            //}

            if (!ModelState.IsValid)
            {
                try
                {
                    _context.Update(resident);
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
            return View(resident);
        }

        // GET: Residents/Delete/5
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

            return View(resident);
        }

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
        public async Task<IActionResult> Current()
        {
            var currentRenters = await _context.Residents
                .Where(r => r.IsCurrentlyLiving == true)
                .ToListAsync();
            return View(currentRenters);
        }

        // GET: Residents/Past
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
