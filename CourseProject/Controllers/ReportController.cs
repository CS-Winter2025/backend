using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseProject.Common;

namespace CourseProject.Controllers
{
    public class ReportController : Controller
    {
        private readonly DatabaseContext _context;

        public ReportController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var residents = await _context.Residents
                        .Include(r => r.EventSchedules)
                        .ToListAsync();

            foreach (var resident in residents)
            {
                resident.Services = await _context.Services
                    .Where(s => resident.ServiceSubscriptionIds.Contains(s.ServiceID))
                    .ToListAsync();
            }

            ViewBag.ParsedDetails = residents.ToDictionary(
                r => r.ResidentId,
                r => Util.ParseJson(r.DetailsJson ?? string.Empty) ?? new()
            );

            return View(residents);
        }
    }
}