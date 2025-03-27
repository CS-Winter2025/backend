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
        public IActionResult Get()
        {
            var data = _context.EventSchedules
                .Include(e => e.Employees)
                .Include(e => e.Service)
                .ToList()
                .Select(e => (WebAPIEvent)e);

            var services = _context.Services
                .Select(s => new
                {
                    value = s.ServiceID,
                    label = s.Type
                })
                .ToList();

            return Ok(new { data, collections = new { services } });
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
            if (apiEvent == null || string.IsNullOrEmpty(apiEvent.employee_ids))
            {
                return BadRequest("Invalid event data or missing employee IDs.");
            }

            // Parse employeeIds into a list of integers
            var employeeIdList = apiEvent.employee_ids
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => int.TryParse(id.Trim(), out var parsedId) ? parsedId : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToList();

            if (!employeeIdList.Any())
            {
                return BadRequest("No valid employee IDs provided.");
            }

            // Find the first valid employee ID in the database
            var validEmployeeId = _context.Employees
                .Where(e => employeeIdList.Contains(e.EmployeeId))
                .Select(e => e.EmployeeId)
                .FirstOrDefault();

            if (validEmployeeId == 0)
            {
                return BadRequest("No valid employee ID found in the database.");
            }

            var newEvent = (EventSchedule)apiEvent;
            //newEvent.EmployeeID = validEmployeeId;

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
            if (apiEvent == null || string.IsNullOrEmpty(apiEvent.employee_ids))
            {
                return BadRequest("Invalid event data or missing employee IDs.");
            }

            // Parse employeeIds into a list of integers
            var employeeIdList = apiEvent.employee_ids
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => int.TryParse(id.Trim(), out var parsedId) ? parsedId : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToList();

            if (!employeeIdList.Any())
            {
                return BadRequest("No valid employee IDs provided.");
            }

            // Find the first valid employee ID in the database
            var validEmployeeId = _context.Employees
                .Where(e => employeeIdList.Contains(e.EmployeeId))
                .Select(e => e.EmployeeId)
                .FirstOrDefault();

            if (validEmployeeId == 0)
            {
                return BadRequest("No valid employee ID found in the database.");
            }

            var updatedEvent = (EventSchedule)apiEvent;
            //updatedEvent.EmployeeID = validEmployeeId;
            var dbEvent = _context.EventSchedules.Find(id);
            if (dbEvent == null)
            {
                return null;
            }
            updatedEvent.Service = _context.Services.Find(apiEvent.service_id);
            dbEvent.Service = _context.Services.Find(dbEvent.ServiceID);
            dbEvent.Service.Type = updatedEvent.Service.Type;
            //dbEvent.EmployeeID = updatedEvent.EmployeeID;
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
