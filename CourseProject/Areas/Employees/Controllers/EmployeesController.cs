using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CourseProject;
using CourseProject.Models;

namespace CourseProject.Areas.Employees.Controllers
{
    [Area("Employees")]
    public class EmployeesController : Controller
    {
        private readonly DatabaseContext _context;

        public EmployeesController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Employees/Employees
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Users"); // Redirect to login if not logged in
            }
            var databaseContext = _context.Employees.Include(e => e.Manager).Include(e => e.Organization);
            return View(await databaseContext.ToListAsync());
        }

        // GET: Employees/Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Manager)
                .Include(e => e.Organization)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Employees/Create
        public IActionResult Create()
        {
            ViewBag.ManagerId = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
            ViewBag.OrganizationId = new SelectList(_context.Organizations, "OrganizationId", "OrganizationId");
            return View();
        }


        // POST: Employees/Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,ManagerId,JobTitle,EmploymentType,PayRate,Availability,HoursWorked,Certifications,OrganizationId,Name,DetailsJson, ProfilePicture")] Employee employee, string Certifications, IFormFile profilePicture)
        {
            if (!ModelState.IsValid)
            {
                // Convert the string input into a list
                employee.Certifications = Certifications?.Split(',').Select(c => c.Trim()).ToList() ?? new List<string>();

                // Handle profile picture upload
                if (profilePicture != null && profilePicture.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await profilePicture.CopyToAsync(memoryStream);
                        employee.ProfilePicture = memoryStream.ToArray();
                    }
                }

                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ManagerId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", employee.ManagerId);
            ViewData["OrganizationId"] = new SelectList(_context.Organizations, "OrganizationId", "OrganizationId", employee.OrganizationId);

            return View(employee);
        }


        // GET: Employees/Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            

            // Pass the data to the view
            ViewData["ManagerId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", employee.ManagerId);
            ViewData["OrganizationId"] = new SelectList(_context.Organizations, "OrganizationId", "OrganizationId", employee.OrganizationId);

            ViewBag.Availability = string.Join(", ", employee.Availability);
            ViewBag.Certifications = string.Join(", ", employee.Certifications);
            ViewBag.HoursWorked = string.Join(", ", employee.HoursWorked);
            return View(employee);
        }

        // POST: Employees/Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,ManagerId,JobTitle,EmploymentType,PayRate,Availability,HoursWorked,Certifications,OrganizationId,Name,DetailsJson")] Employee employee, IFormFile profilePicture)
        {
            //if (id != employee.EmployeeId)
            //{
            //    return NotFound();
            //}

            if (!ModelState.IsValid)
            {
                try
                {

                    var existingEmployee = await _context.Employees.FindAsync(id);

                    if (existingEmployee == null)
                    {
                        return NotFound();
                    }

                    if (profilePicture != null && profilePicture.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await profilePicture.CopyToAsync(memoryStream);
                            existingEmployee.ProfilePicture = memoryStream.ToArray();
                        }
                    }

                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeId))
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
            ViewData["ManagerId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", employee.ManagerId);
            ViewData["OrganizationId"] = new SelectList(_context.Organizations, "OrganizationId", "OrganizationId", employee.OrganizationId);
            return View(employee);
        }

        // GET: Employees/Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Manager)
                .Include(e => e.Organization)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                Console.WriteLine($"Employee with ID {id} not found!");
                return RedirectToAction(nameof(Index)); // Ensure you're not just silently failing
            }

            Console.WriteLine($"Deleting Employee: {employee.EmployeeId} - {employee.Name}");

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
