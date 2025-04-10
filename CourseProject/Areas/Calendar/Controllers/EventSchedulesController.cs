using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CourseProject;
using CourseProject.Models;
using CourseProject.Common;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
        [Authorize(Roles =
            nameof(UserRole.ADMIN) + "," +
            nameof(UserRole.RESIDENT) + "," +
            nameof(UserRole.EMPLOYEE) + "," +
            nameof(UserRole.HOUSING_MANAGER) + "," +
            nameof(UserRole.HR_MANAGER)
        )]
        public async Task<IActionResult> Index(int? userId, string? userType)
        {
            if (userType == "employee" && userId != null)
            {
                var employee = await _context.Employees
                    .Include(e => e.Name)
                    .FirstOrDefaultAsync(e => e.EmployeeId == userId);

                ViewData["UserID"] = userId;
                ViewData["UserType"] = userType;
                ViewData["UserName"] = employee.Name.ToString();

                var empDatabaseContext = _context.EventSchedules.Include(e => e.Employees).Include(e => e.Service);
                return View(await empDatabaseContext.ToListAsync());
            }

            if (userType == "resident" && userId != null)
            {
                var resident = await _context.Residents
                    .Include(r => r.Name)
                    .FirstOrDefaultAsync(r => r.ResidentId == userId);

                ViewData["UserID"] = userId;
                ViewData["UserType"] = userType;
                ViewData["UserName"] = resident.Name.ToString();

                var resDatabaseContext = _context.EventSchedules.Include(e => e.Employees).Include(e => e.Service);
                return View(await resDatabaseContext.ToListAsync());
            }

            if (userId == null)
            {
                string? stringId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (stringId == null) return RedirectToAction("NotFound", "Error");
                userId = Int32.Parse(stringId);
            }

            User? user = await _context.Users.Include(u => u.Resident)
                                .Include(u => u.Employee)
                                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || user.Id == null || user.Role == null || user.Role == UserRole.NONE)
            {
                return RedirectToAction("NotFound", "Error");
            }

            if (user.Role == UserRole.EMPLOYEE && user.EmployeeId == null)
            {
                return RedirectToAction("NotFound", "Error");
            }

            if (user.Role == UserRole.RESIDENT && user.ResidentId == null)
            {
                return RedirectToAction("NotFound", "Error");
            }

            string userName;
            if (user.Role == UserRole.EMPLOYEE) 
            {
                userName = user.Employee.Name.ToString();
            } 
            else if (user.Role == UserRole.RESIDENT)
            {
                userName = user.Resident.Name.ToString();
            } 
            else
            {
                userName = user.Role.ToString();
            }

            ViewData["UserID"] = userId;
            ViewData["UserType"] = "";
            ViewData["UserName"] = userName;

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
