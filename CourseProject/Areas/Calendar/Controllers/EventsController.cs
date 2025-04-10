using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CourseProject;
using CourseProject.Models;
using System.Security.Claims;
using System.Dynamic;

namespace CourseProject.Areas.Calendar.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public EventsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET api/events
        [HttpGet]
        public IActionResult Get(int? userId, string? userType)
        {
            int? employeeId = null;
            if (userType == "employee")
            {
                employeeId = userId;
            }
            int? residentId = null;
            if (userType == "resident")
            {
                residentId = userId;
            }

            string? stringId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (stringId == null) return RedirectToAction("NotFound", "Error");
            userId = Int32.Parse(stringId);

            User? user = _context.Users.Find(userId);

            if (employeeId == null && residentId == null)
            {
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
            }

            var fullTable = _context.ScheduleBase
                .OfType<EventSchedule>()
                .Include(e => e.Employees) // Ensure Employees are included
                .Include(e => e.Service)
                .Include(e => e.Resident);

            IQueryable<EventSchedule> userTable = Enumerable.Empty<EventSchedule>().AsQueryable();
            if (userType == null)
            {
                if (user.Role == UserRole.ADMIN || user.Role == UserRole.HOUSING_MANAGER || user.Role == UserRole.HR_MANAGER)
                {
                    userTable = fullTable;
                }
                else if (user.Role == UserRole.EMPLOYEE)
                {
                    userTable = fullTable
                        .Where(e => e.Employees.Any(emp => emp.EmployeeId == user.EmployeeId));
                }
                else if (user.Role == UserRole.RESIDENT)
                {
                    userTable = fullTable
                        .Where(e => e.ResidentId == user.ResidentId);
                }
            }
            else if (userType == "employee")
            {
                userTable = fullTable
                        .Where(e => e.Employees.Any(emp => emp.EmployeeId == employeeId));
            }
            else if (userType == "resident")
            {
                userTable = fullTable
                        .Where(e => e.ResidentId == residentId);
            }


                var data = userTable
            .ToList()
            .Select(e => new WebAPIEvent
            {
                id = e.ScheduleBaseId,
                text = e.Service.Type,
                start_date = e.StartDate.ToString("yyyy-MM-dd HH:mm"),
                end_date = e.EndDate.ToString("yyyy-MM-dd HH:mm"),
                rrule = e.RepeatPattern,
                duration = e.Duration,
                recurring_event_id = e.RecurringEventId,
                original_start = e.OriginalStart.ToString("yyyy-MM-dd HH:mm"),
                deleted = e.Deleted,
                service_id = e.ServiceID,
                resident_id = e.ResidentId,
                status = e.Status,
                // Convert the list of Employee IDs to a comma-separated string
                employee_ids = string.Join(",", e.Employees.Select(emp => emp.EmployeeId.ToString()))
            });

            dynamic collections = new ExpandoObject();

            collections.services = _context.Services
                .Select(s => new
                {
                    value = s.ServiceID,
                    label = s.Type
                })
                .ToList();

            collections.residents = _context.Residents
                .AsNoTracking()
                .Select(r => new
                {
                    value = r.ResidentId,
                    label = r.Name.ToString()
                })
                .ToList();

            collections.employees = _context.Employees
                .AsNoTracking()
                .Select(e => new
                {
                    value = e.EmployeeId,
                    label = e.Name.ToString()
                })
                .ToList();
            
            collections.role = new List<object> { new { value = 1, label = user.Role.ToString() } };

            return Ok(new { data, collections });
        }

        // GET api/events/5
        [HttpGet("{id}")]
        public EventSchedule? Get(int id)
        {
            return _context
                .EventSchedules
                .Find(id);
        }

        // POST api/events
        [HttpPost]
        public ObjectResult Post([FromBody] WebAPIEvent apiEvent)
        {
            // Parse employeeIds into a list of integers
            var employeeIdList = apiEvent.employee_ids
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => int.TryParse(id.Trim(), out var parsedId) ? parsedId : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToList();

            var employees = _context.Employees
                .Where(e => employeeIdList.Contains(e.EmployeeId)) // Find employees with the given IDs
                .ToList();

            var newEvent = (EventSchedule)apiEvent;
            //newEvent.EmployeeID = validEmployeeId;
            newEvent.Employees = employees;

            _context.EventSchedules.Add(newEvent);
            _context.SaveChanges();

            return Ok(new
            {
                tid = newEvent.ScheduleBaseId,
                action = "inserted"
            });
        }

        // PUT api/events/5
        [HttpPut("{id}")]
        public ObjectResult? Put(int id, [FromBody] WebAPIEvent apiEvent)
        {
            // Parse employeeIds into a list of integers
            var employeeIdList = apiEvent.employee_ids
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => int.TryParse(id.Trim(), out var parsedId) ? parsedId : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToList();

            var employees = _context.Employees
                .Where(e => employeeIdList.Contains(e.EmployeeId)) // Find employees with the given IDs
                .ToList();

            var updatedEvent = (EventSchedule)apiEvent;
            //updatedEvent.EmployeeID = validEmployeeId;
            updatedEvent.Employees = employees;
            var dbEvent = _context.EventSchedules.Include(e => e.Employees).FirstOrDefault(e => e.ScheduleBaseId == id);
            if (dbEvent == null)
            {
                return null;
            }

            updatedEvent.Service = _context.Services.Find(apiEvent.service_id);
            updatedEvent.Resident = _context.Residents.Find(apiEvent.resident_id);
            //dbEvent.Service = _context.Services.Find(dbEvent.ServiceID);
            dbEvent.Service = updatedEvent.Service;
            dbEvent.Resident = updatedEvent.Resident;
            //dbEvent.EmployeeID = updatedEvent.EmployeeID;
            dbEvent.Employees.Clear();
            dbEvent.Employees = updatedEvent.Employees;
            dbEvent.StartDate = updatedEvent.StartDate;
            dbEvent.EndDate = updatedEvent.EndDate;
            dbEvent.RepeatPattern = updatedEvent.RepeatPattern;
            dbEvent.RecurringEventId = updatedEvent.RecurringEventId;
            dbEvent.Status = updatedEvent.Status;
            _context.SaveChanges();

            return Ok(new
            {
                action = "updated"
            });
        }

        // DELETE api/events/5
        [HttpDelete("{id}")]
        public ObjectResult DeleteEvent(int id)
        {
            var e = _context.EventSchedules.Find(id);
            if (e != null)
            {
                _context.EventSchedules.Remove(e);
                _context.SaveChanges();
            }

            return Ok(new
            {
                action = "deleted"
            });
        }
    }
}
