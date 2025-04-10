using CourseProject;
using CourseProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Area("Housing")]
public class InvoicesController : Controller
{
    private readonly DatabaseContext _context;

    public InvoicesController(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> MonthlyInvoices(int? month, int? year)
    {
        var targetMonth = month ?? DateTime.Now.Month;
        var targetYear = year ?? DateTime.Now.Year;

        var startDate = new DateTime(targetYear, targetMonth, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var assignments = await _context.ResidentAssets
            .Include(ra => ra.Resident)
            .Include(ra => ra.Asset)
            .Where(ra => ra.ToDate >= startDate && ra.FromDate <= endDate)
            .ToListAsync();

        var invoices = assignments
            .GroupBy(ra => ra.Resident)
            .Select(group =>
            {
                var items = group.Select(ra =>
                {
                    var from = ra.FromDate < startDate ? startDate : ra.FromDate;
                    var to = ra.ToDate > endDate ? endDate : ra.ToDate;
                    var days = (to - from).Days + 1;

                    return new InvoiceItemViewModel
                    {
                        AssetType = ra.Asset.Type,
                        Days = days,
                        DailyRate = ra.Asset.Price,
                        Total = days * ra.Asset.Price
                    };
                }).ToList();

                return new ResidentInvoiceViewModel
                {
                    Resident = group.Key,
                    Items = items,
                    TotalDue = items.Sum(i => i.Total)
                };
            }).ToList();

        ViewBag.Month = targetMonth;
        ViewBag.Year = targetYear;

        return View(invoices);
    }
}
