using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
                .Include(r => r.Services)
                .ToListAsync();

            ViewBag.ParsedDetails = ParseResidentDetails(residents);

            return View(residents);
        }
        private static Dictionary<int, Dictionary<string, object>> ParseResidentDetails(IEnumerable<Resident> residents)
        {
            return residents.ToDictionary(
                r => r.ResidentId,
                r => JsonConvert.DeserializeObject<Dictionary<string, object>>(r.DetailsJson ?? "{}") ?? []
            );
        }
    }
}