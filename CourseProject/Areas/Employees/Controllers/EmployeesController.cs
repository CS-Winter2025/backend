using System.Security.Claims;
using CourseProject.Common;
using CourseProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HR_MANAGER) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> Index()
        {
            var databaseContext = _context.Employees.Include(e => e.Manager).Include(e => e.Organization);
            return View(await databaseContext.ToListAsync());
        }

        // GET: Employees/Employees/Details/5
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HR_MANAGER) + "," + nameof(UserRole.HOUSING_MANAGER))]
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
            ViewBag.Details = employee.DetailsJson != null
                ? Util.ParseJson(employee.DetailsJson)
                : new Dictionary<string, string>();
            return View(employee);
        }

        [Authorize(Roles = nameof(UserRole.EMPLOYEE) + "," + nameof(UserRole.ADMIN))]
        public async Task<IActionResult> Me()
        {
            string? stringId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (stringId == null) return RedirectToAction("Forbidden", "Error");

            int id = Int32.Parse(stringId);
            User? user = await _context.Users
                .Include(u => u.Employee)
                .ThenInclude(e => e.Services)
                .Include(u => u.Employee.Organization)
                .FirstOrDefaultAsync(u => u.Id == id);

            ViewBag.Details = user.Employee.DetailsJson != null
                ? Util.ParseJson(user.Employee.DetailsJson)
                : new Dictionary<string, string>();

            if (user == null) return RedirectToAction("NotFound", "Error");
            return View("Me", user);
        }


        // GET: Employees/Employees/Create
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HR_MANAGER) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public IActionResult Create()
        {
            var employee = new Employee
            {
                Name = new FullName(),
                Address = new FullAddress()
            };

            ViewBag.ManagerId = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
            ViewBag.OrganizationId = new SelectList(_context.Organizations, "OrganizationId", "OrganizationId");
            return View(employee);
        }

        // POST: Employees/Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HR_MANAGER) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> Create([Bind("EmployeeId,ManagerId,JobTitle,EmploymentType,PayRate,Availability,HoursWorked,Certifications,OrganizationId,Name,Address,DetailsJson,ProfilePicture")] Employee employee, string Certifications, IFormFile ProfilePicture)
        {
            if (!ModelState.IsValid)
            {

                if (ProfilePicture != null && ProfilePicture.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await ProfilePicture.CopyToAsync(memoryStream);
                        employee.ProfilePicture = memoryStream.ToArray();  // Convert the file to byte array
                    }
                }
                // Convert the string input into a list
                employee.Certifications = Certifications?.Split(',').Select(c => c.Trim()).ToList() ?? new List<string>();

                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ManagerId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", employee.ManagerId);
            ViewData["OrganizationId"] = new SelectList(_context.Organizations, "OrganizationId", "OrganizationId", employee.OrganizationId);

            return View(employee);
        }

        // GET: Employees/Employees/Edit/5
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HR_MANAGER) + "," + nameof(UserRole.HOUSING_MANAGER))]
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
            ViewBag.Details = employee.DetailsJson != null
                ? Util.ParseJson(employee.DetailsJson)
                : new Dictionary<string, string>();

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
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HR_MANAGER) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,ManagerId,JobTitle,EmploymentType,PayRate,Availability,HoursWorked,Certifications,OrganizationId,Name,Address,DetailsJson")] Employee employee, IFormFile ProfilePicture)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    var existingEmployee = await _context.Employees.FindAsync(id);

                    if (existingEmployee == null)
                    {
                        return NotFound();
                    }

                    if (ProfilePicture != null && ProfilePicture.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await ProfilePicture.CopyToAsync(memoryStream);
                            existingEmployee.ProfilePicture = memoryStream.ToArray();
                        }
                    }
                    existingEmployee.EmployeeId = employee.EmployeeId;
                    existingEmployee.ManagerId = employee.ManagerId;
                    existingEmployee.JobTitle = employee.JobTitle;
                    existingEmployee.EmploymentType = employee.EmploymentType;
                    existingEmployee.PayRate = employee.PayRate;
                    existingEmployee.Availability = employee.Availability;
                    existingEmployee.HoursWorked = employee.HoursWorked;
                    existingEmployee.Certifications = employee.Certifications;
                    existingEmployee.OrganizationId = employee.OrganizationId;
                    existingEmployee.Name = employee.Name;
                    existingEmployee.Address = employee.Address;
                    existingEmployee.DetailsJson = employee.DetailsJson;

                    _context.Update(existingEmployee);

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
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HR_MANAGER) + "," + nameof(UserRole.HOUSING_MANAGER))]
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
            ViewBag.Details = employee.DetailsJson != null
                ? Util.ParseJson(employee.DetailsJson)
                : new Dictionary<string, string>();

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HR_MANAGER) + "," + nameof(UserRole.HOUSING_MANAGER))]
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
