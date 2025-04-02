using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CourseProject;
using CourseProject.Models;
using CourseProject.Common;
using Microsoft.AspNetCore.Authorization;

namespace CourseProject.Areas.Charges.Controllers
{
    [Area("Charges")]
    public class InvoicesController : Controller
    {
        private readonly DatabaseContext _context;

        public InvoicesController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Charges/Invoices
        [Authorize(Roles = nameof(UserRole.RESIDENT) + "," + nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> Index()
        {
            var invoices = _context.Invoices.Include(i => i.Resident);
            return View(await invoices.ToListAsync());
        }

        // GET: Charges/Invoices/Details/5
        [Authorize(Roles = nameof(UserRole.RESIDENT) + "," + nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.Resident)
                .FirstOrDefaultAsync(m => m.InvoiceID == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // GET: Charges/Invoices/Create
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public IActionResult Create()
        {
            ViewData["ResidentID"] = new SelectList(_context.Residents, "ResidentId", "ResidentId");
            return View();
        }

        // POST: Charges/Invoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> Create([Bind("InvoiceID,ResidentID,Date,AmountDue,AmountPaid")] Invoice invoice)
        {
            if (!Util.HasAccess(HttpContext, UserRole.ADMIN, UserRole.HOUSING_MANAGER, UserRole.EMPLOYEE))
                return RedirectToAction("Forbidden", "Error");

            Resident? resident = await _context.Residents.FindAsync(invoice.ResidentID);
            if (resident == null) return View(invoice);
            else ModelState.SetModelValue("Resident", resident, null);

            ModelState.Remove(nameof(invoice.Resident));

            if (ModelState.IsValid)
            {
                _context.Add(invoice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ResidentID"] = new SelectList(_context.Residents, "ResidentId", "ResidentId", invoice.ResidentID);
            return View(invoice);
        }

        // GET: Charges/Invoices/Edit/5
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            ViewData["ResidentID"] = new SelectList(_context.Residents, "ResidentId", "ResidentId", invoice.ResidentID);
            return View(invoice);
        }

        // POST: Charges/Invoices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> Edit([Bind(Prefix = "InvoiceID")] int id, [Bind("InvoiceID,ResidentID,Date,AmountDue,AmountPaid")] Invoice invoice)
        {
            if (!Util.HasAccess(HttpContext, UserRole.ADMIN, UserRole.HOUSING_MANAGER, UserRole.EMPLOYEE))
                return RedirectToAction("Forbidden", "Error");

            if (id != invoice.InvoiceID)
            {
                return NotFound();
            }

            Resident? resident = await _context.Residents.FindAsync(invoice.ResidentID);
            if (resident == null) return View(invoice);
            else ModelState.SetModelValue("Resident", resident, null);

            ModelState.Remove(nameof(invoice.Resident));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvoiceExists(invoice.InvoiceID))
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
            ViewData["ResidentID"] = new SelectList(_context.Residents, "ResidentId", "ResidentId", invoice.ResidentID);
            return View(invoice);
        }

        // GET: Charges/Invoices/Delete/5
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!Util.HasAccess(HttpContext, UserRole.ADMIN, UserRole.HOUSING_MANAGER, UserRole.EMPLOYEE))
                return RedirectToAction("Forbidden", "Error");

            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.Resident)
                .FirstOrDefaultAsync(m => m.InvoiceID == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // POST: Charges/Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> DeleteConfirmed([Bind(Prefix = "InvoiceID")] int id)
        {
            if (!Util.HasAccess(HttpContext, UserRole.ADMIN, UserRole.HOUSING_MANAGER, UserRole.EMPLOYEE))
                return RedirectToAction("Forbidden", "Error");

            Console.WriteLine(id);
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice != null)
            {
                _context.Invoices.Remove(invoice);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InvoiceExists(int id)
        {
            return _context.Invoices.Any(e => e.InvoiceID == id);
        }
    }
}
