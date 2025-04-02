﻿using System;
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

namespace CourseProject.Areas.Services.Controllers
{
    [Area("Services")]
    public class ServicesController : Controller
    {
        private readonly DatabaseContext _context;

        public ServicesController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Services/Services       
        [Authorize(Roles = nameof(UserRole.NONE) + "," +
                           nameof(UserRole.EMPLOYEE) + "," +
                           nameof(UserRole.RESIDENT) + "," +
                           nameof(UserRole.HR_MANAGER) + "," +
                           nameof(UserRole.HOUSING_MANAGER) + "," +
                           nameof(UserRole.ADMIN))]
        public async Task<IActionResult> Index()
        {
            if (Util.HasAccess(HttpContext, UserRole.ADMIN, UserRole.SERVICE_MANAGER, UserRole.EMPLOYEE))
            {
                return View(await _context.Services.ToListAsync());
            }
            return RedirectToAction("Forbidden", "Error");
        }

        // GET: Services/Services/Details/5
        // All roles can read
        [Authorize(Roles = nameof(UserRole.NONE) + "," +
                           nameof(UserRole.EMPLOYEE) + "," +
                           nameof(UserRole.RESIDENT) + "," +
                           nameof(UserRole.HR_MANAGER) + "," +
                           nameof(UserRole.HOUSING_MANAGER) + "," +
                           nameof(UserRole.ADMIN))]
        public async Task<IActionResult> Details(int? id)
        {
            if (!Util.HasAccess(HttpContext, UserRole.ADMIN, UserRole.SERVICE_MANAGER, UserRole.EMPLOYEE)) 
                return RedirectToAction("Forbidden", "Error");

            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .FirstOrDefaultAsync(m => m.ServiceID == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        // GET: Services/Services/Create
        [Authorize(Roles = nameof(UserRole.HOUSING_MANAGER) + "," + nameof(UserRole.ADMIN))]
        public IActionResult Create()
        {            
            if (!Util.HasAccess(HttpContext, UserRole.ADMIN, UserRole.SERVICE_MANAGER, UserRole.EMPLOYEE))
                return RedirectToAction("Forbidden", "Error");

            return View();
        }

        // POST: Services/Services/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(UserRole.HOUSING_MANAGER) + "," + nameof(UserRole.ADMIN))]
        public async Task<IActionResult> Create([Bind("ServiceID,Type,Rate,Requirements")] Service service)
        {
            if (!Util.HasAccess(HttpContext, UserRole.ADMIN, UserRole.SERVICE_MANAGER, UserRole.EMPLOYEE))
                return RedirectToAction("Forbidden", "Error");

            if (ModelState.IsValid)
            {
                _context.Add(service);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(service);
        }

        // GET: Services/Services/Edit/5
        [Authorize(Roles = nameof(UserRole.HR_MANAGER) + "," +
                           nameof(UserRole.HOUSING_MANAGER) + "," +
                           nameof(UserRole.ADMIN))]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }
            return View(service);
        }

        // POST: Services/Services/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(UserRole.HR_MANAGER) + "," +
                           nameof(UserRole.HOUSING_MANAGER) + "," +
                           nameof(UserRole.ADMIN))]
        public async Task<IActionResult> Edit([Bind(Prefix = "ServiceID")] int id, [Bind("ServiceID,Type,Rate,Requirements")] Service service)
        {
            if (!Util.HasAccess(HttpContext, UserRole.ADMIN, UserRole.SERVICE_MANAGER, UserRole.EMPLOYEE))
                return RedirectToAction("Forbidden", "Error");

            if (id != service.ServiceID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(service);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceExists(service.ServiceID))
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
            return View(service);
        }

        // GET: Services/Services/Delete/5
        [Authorize(Roles = nameof(UserRole.HOUSING_MANAGER) + "," + nameof(UserRole.ADMIN))]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!Util.HasAccess(HttpContext, UserRole.ADMIN, UserRole.SERVICE_MANAGER, UserRole.EMPLOYEE))
                return RedirectToAction("Forbidden", "Error");

            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .FirstOrDefaultAsync(m => m.ServiceID == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        // POST: Services/Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(UserRole.HOUSING_MANAGER) + "," + nameof(UserRole.ADMIN))]
        public async Task<IActionResult> DeleteConfirmed([Bind(Prefix = "ServiceID")] int id)
        {
            if (!Util.HasAccess(HttpContext, UserRole.ADMIN, UserRole.SERVICE_MANAGER, UserRole.EMPLOYEE))
                return RedirectToAction("Forbidden", "Error");

            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);                
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool ServiceExists(int id)
        {
            return _context.Services.Any(e => e.ServiceID == id);
        }        
    }
}
