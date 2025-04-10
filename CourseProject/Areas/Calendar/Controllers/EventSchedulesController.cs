using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CourseProject;
using CourseProject.Models;

namespace CourseProject.Areas.Calendar.Controllers
{
    [Area("Calendar")]
    public class EventSchedulesController : Controller
    {
        private readonly DatabaseContext _context;

        public EventSchedulesController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Calendar/EventSchedules
        public async Task<IActionResult> Index(int? personId, bool? isEmployee)
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Users"); // Redirect to login if not logged in
            }
            string personName = string.Empty;

            if (personId.HasValue && isEmployee.HasValue)
            {
                if ((bool)isEmployee)
                {
                    var employee = _context.Employees.FirstOrDefault(e => e.EmployeeId == personId.Value);
                    if (employee != null)
                    {
                        personName = employee.Name.ToString();
                    }
                }
                else
                {
                    var resident = _context.Residents.FirstOrDefault(e => e.ResidentId == personId.Value);
                    if (resident != null)
                    {
                        personName = resident.Name.ToString();
                    }
                }

            }

            ViewData["PersonID"] = personId;
            ViewData["PersonName"] = personName;
            ViewData["IsEmployee"] = isEmployee.ToString();


            var databaseContext = _context.EventSchedules.Include(e => e.Employees).Include(e => e.Service);
            return View(await databaseContext.ToListAsync());
        }

        // GET: Calendar/EventSchedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventSchedule = await _context.EventSchedules
                .Include(e => e.Employees)
                .Include(e => e.Service)
                .FirstOrDefaultAsync(m => m.EventScheduleId == id);
            if (eventSchedule == null)
            {
                return NotFound();
            }

            return View(eventSchedule);
        }

        // GET: Calendar/EventSchedules/Create
        public IActionResult Create()
        {
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
            ViewData["ServiceID"] = new SelectList(_context.Services, "ServiceID", "ServiceID");
            return View();
        }

        // POST: Calendar/EventSchedules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventScheduleId,EmployeeID,ServiceID,RangeOfHours,StartDate,EndDate,RepeatPattern")] EventSchedule eventSchedule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(eventSchedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["EmployeeID"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", eventSchedule.EmployeeID);
            ViewData["ServiceID"] = new SelectList(_context.Services, "ServiceID", "ServiceID", eventSchedule.ServiceID);
            return View(eventSchedule);
        }

        // GET: Calendar/EventSchedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventSchedule = await _context.EventSchedules.FindAsync(id);
            if (eventSchedule == null)
            {
                return NotFound();
            }
            //ViewData["EmployeeID"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", eventSchedule.EmployeeID);
            ViewData["ServiceID"] = new SelectList(_context.Services, "ServiceID", "ServiceID", eventSchedule.ServiceID);
            return View(eventSchedule);
        }

        // POST: Calendar/EventSchedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventScheduleId,EmployeeID,ServiceID,RangeOfHours,StartDate,EndDate,RepeatPattern")] EventSchedule eventSchedule)
        {
            if (id != eventSchedule.EventScheduleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventSchedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventScheduleExists(eventSchedule.EventScheduleId))
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
            //ViewData["EmployeeID"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", eventSchedule.EmployeeID);
            ViewData["ServiceID"] = new SelectList(_context.Services, "ServiceID", "ServiceID", eventSchedule.ServiceID);
            return View(eventSchedule);
        }

        // GET: Calendar/EventSchedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventSchedule = await _context.EventSchedules
                .Include(e => e.Employees)
                .Include(e => e.Service)
                .FirstOrDefaultAsync(m => m.EventScheduleId == id);
            if (eventSchedule == null)
            {
                return NotFound();
            }

            return View(eventSchedule);
        }

        // POST: Calendar/EventSchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventSchedule = await _context.EventSchedules.FindAsync(id);
            if (eventSchedule != null)
            {
                _context.EventSchedules.Remove(eventSchedule);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventScheduleExists(int id)
        {
            return _context.EventSchedules.Any(e => e.EventScheduleId == id);
        }
    }
}
