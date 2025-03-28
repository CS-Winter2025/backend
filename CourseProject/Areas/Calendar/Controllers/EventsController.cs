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
        public IActionResult Get(int? employeeId)
        {
            //var residentEvents = _context.Residents
            //    .Include(e => e.EventSchedules)
            //    .ThenInclude(e => e.Employees)
            //    .Include(e => e.EventSchedules)
            //    .ThenInclude(e => e.Service)
            //    .Where(e => e.Employees.Any(emp => emp.EmployeeId == employeeId))
            //    .ToList()
            //    .Select(e => new WebAPIEvent
            //    {
            //        id = e.EventScheduleId,
            //        text = e.Service.Type,
            //        start_date = e.StartDate.ToString("yyyy-MM-dd HH:mm"),
            //        end_date = e.EndDate.ToString("yyyy-MM-dd HH:mm"),
            //        service_id = e.ServiceID,
            //        // Convert the list of Employee IDs to a comma-separated string
            //        employee_ids = string.Join(",", e.Employees.Select(emp => emp.EmployeeId.ToString()))
            //    });

            //Console.WriteLine("IN GET: " + employeeId);
            var data = _context.EventSchedules
            .Include(e => e.Employees) // Ensure Employees are included
            .Include(e => e.Service)
            .Include(e => e.Resident)
            .Where(e => e.Employees.Any(emp => emp.EmployeeId == employeeId))
            .ToList()
            .Select(e => new WebAPIEvent
            {
                id = e.EventScheduleId,
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
                // Convert the list of Employee IDs to a comma-separated string
                employee_ids = string.Join(",", e.Employees.Select(emp => emp.EmployeeId.ToString()))
            });

            var services = _context.Services
                .Select(s => new
                {
                    value = s.ServiceID,
                    label = s.Type
                })
                .ToList();

            var residents = _context.Residents
                .Select(r => new
                {
                    value = r.ResidentId,
                    label = r.Name
                })
                .ToList();

            return Ok(new { data, collections = new { services, residents } });
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
            //if (apiEvent == null || string.IsNullOrEmpty(apiEvent.employee_ids))
            //{
            //    return BadRequest("Invalid event data or missing employee IDs.");
            //}

            //// Parse employeeIds into a list of integers
            //var employeeIdList = apiEvent.employee_ids
            //    .Split(',', StringSplitOptions.RemoveEmptyEntries)
            //    .Select(id => int.TryParse(id.Trim(), out var parsedId) ? parsedId : (int?)null)
            //    .Where(id => id.HasValue)
            //    .Select(id => id.Value)
            //    .ToList();

            //if (!employeeIdList.Any())
            //{
            //    return BadRequest("No valid employee IDs provided.");
            //}

            //// Find the first valid employee ID in the database
            //var validEmployeeId = _context.Employees
            //    .Where(e => employeeIdList.Contains(e.EmployeeId))
            //    .Select(e => e.EmployeeId)
            //    .FirstOrDefault();

            //if (validEmployeeId == 0)
            //{
            //    return BadRequest("No valid employee ID found in the database.");
            //}

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
                tid = newEvent.EventScheduleId,
                action = "inserted"
            });
        }

        // PUT api/events/5
        [HttpPut("{id}")]
        public ObjectResult? Put(int id, [FromBody] WebAPIEvent apiEvent)
        {
            //if (apiEvent == null || string.IsNullOrEmpty(apiEvent.employee_ids))
            //{
            //    return BadRequest("Invalid event data or missing employee IDs.");
            //}

            //// Parse employeeIds into a list of integers
            //var employeeIdList = apiEvent.employee_ids
            //    .Split(',', StringSplitOptions.RemoveEmptyEntries)
            //    .Select(id => int.TryParse(id.Trim(), out var parsedId) ? parsedId : (int?)null)
            //    .Where(id => id.HasValue)
            //    .Select(id => id.Value)
            //    .ToList();

            //if (!employeeIdList.Any())
            //{
            //    return BadRequest("No valid employee IDs provided.");
            //}

            //// Find the first valid employee ID in the database
            //var validEmployeeId = _context.Employees
            //    .Where(e => employeeIdList.Contains(e.EmployeeId))
            //    .Select(e => e.EmployeeId)
            //    .FirstOrDefault();

            //if (validEmployeeId == 0)
            //{
            //    return BadRequest("No valid employee ID found in the database.");
            //}

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
            var dbEvent = _context.EventSchedules.Include(e => e.Employees).FirstOrDefault(e => e.EventScheduleId == id);
            if (dbEvent == null)
            {
                return null;
            }

            updatedEvent.Service = _context.Services.Find(apiEvent.service_id);
            dbEvent.Service = _context.Services.Find(dbEvent.ServiceID);
            dbEvent.Service = updatedEvent.Service;
            //dbEvent.EmployeeID = updatedEvent.EmployeeID;
            dbEvent.Employees.Clear();
            dbEvent.Employees = updatedEvent.Employees;
            dbEvent.StartDate = updatedEvent.StartDate;
            dbEvent.EndDate = updatedEvent.EndDate;
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
