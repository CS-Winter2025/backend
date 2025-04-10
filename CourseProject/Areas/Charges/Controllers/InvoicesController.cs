﻿using CourseProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
            ViewData["ResidentId"] = new SelectList(_context.Residents, "ResidentId", "ResidentId");
            return View();
        }

        // POST: Charges/Invoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> Create([Bind("InvoiceID,ResidentId,Date,AmountDue,AmountPaid")] Invoice invoice)
        {
            Resident? resident = await _context.Residents.FindAsync(invoice.ResidentId);
            if (resident == null) return View(invoice);
            else ModelState.SetModelValue("Resident", resident, null);

            ModelState.Remove(nameof(invoice.Resident));

            if (ModelState.IsValid)
            {
                _context.Add(invoice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ResidentId"] = new SelectList(_context.Residents, "ResidentId", "ResidentId", invoice.ResidentId);
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
            ViewData["ResidentId"] = new SelectList(_context.Residents, "ResidentId", "ResidentId", invoice.ResidentId);
            return View(invoice);
        }

        // POST: Charges/Invoices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> Edit([Bind(Prefix = "InvoiceID")] int id, [Bind("InvoiceID,ResidentId,Date,AmountDue,AmountPaid")] Invoice invoice)
        {
            if (id != invoice.InvoiceID)
            {
                return NotFound();
            }

            Resident? resident = await _context.Residents.FindAsync(invoice.ResidentId);
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
            ViewData["ResidentId"] = new SelectList(_context.Residents, "ResidentId", "ResidentId", invoice.ResidentId);
            return View(invoice);
        }

        // GET: Charges/Invoices/Delete/5
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Charges/Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(UserRole.ADMIN) + "," + nameof(UserRole.HOUSING_MANAGER))]
        public async Task<IActionResult> DeleteConfirmed([Bind(Prefix = "InvoiceID")] int id)
        {
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
