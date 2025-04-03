using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            return View(residents);
        }
    }
}
