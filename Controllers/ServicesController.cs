using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HubnyxQMS.Data;
using HubnyxQMS.Models;
using Microsoft.AspNetCore.Authorization;

namespace HubnyxQMS.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Services
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Services.Include(s => s.Section);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Services/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .Include(s => s.Section)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        // GET: Services/Create
        public IActionResult Create()
        {
            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Name");
            return View();
        }

        // POST: Services/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,SectionId")] Service service)
        {
            if (ModelState.IsValid)
            {
                var check = await _context.Sections.FirstOrDefaultAsync(c => c.Name == service.Name);
                if (check != null)
                {
                    ViewBag.Error = "Name Exists";
                    return RedirectToAction(nameof(Index));
                }
                Guid gg = Guid.NewGuid();
                var getuniqidd = gg;
                service.Id = getuniqidd.ToString();
                service.Name = service.Name.ToUpper();
                _context.Add(service);
                await _context.SaveChangesAsync();

                //update service REPORT
                ServicesReport servicesReport = new ServicesReport();
                Guid ggg = Guid.NewGuid();
                var getuniqiddd = ggg;
                servicesReport.Id = getuniqiddd.ToString();
                servicesReport.Created  = DateTime.UtcNow.AddHours(3).ToString("dd/MM/yyyy");
                servicesReport.Date = DateTime.UtcNow.AddHours(3);
                servicesReport.ServiceId = getuniqidd.ToString();
                _context.Add(servicesReport);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Services");
            }
            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Id", service.SectionId);
            return View(service);
        }

        // GET: Services/Edit/5
        public async Task<IActionResult> Edit(string id)
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
            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Name", service.SectionId);
            return View(service);
        }

        // POST: Services/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,SectionId")] Service service)
        {
            if (id != service.Id)
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
                    if (!ServiceExists(service.Id))
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
            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "Id", service.SectionId);
            return View(service);
        }

        // GET: Services/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .Include(s => s.Section)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var service = await _context.Services.FindAsync(id);
            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceExists(string id)
        {
            return _context.Services.Any(e => e.Id == id);
        }
    }
}
