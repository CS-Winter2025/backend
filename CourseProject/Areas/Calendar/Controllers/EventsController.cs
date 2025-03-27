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
        public IEnumerable<WebAPIEvent> Get()
        {
            return _context.EventSchedules
                .Include(e => e.Employee)
                .Include(e => e.Service)
                .ToList()
                .Select(e => (WebAPIEvent)e);
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
            var newEvent = (EventSchedule)apiEvent;

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
            var updatedEvent = (EventSchedule)apiEvent;
            var dbEvent = _context.EventSchedules.Find(id);
            if (dbEvent == null)
            {
                return null;
            }
            dbEvent.Service.Type = updatedEvent.Service.Type;
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
