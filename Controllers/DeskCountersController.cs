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
    public class DeskCountersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DeskCountersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DeskCounters
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.DeskCounters.Include(d => d.QMSUser);
            return View(await applicationDbContext.OrderBy(d => d.CounterNumber).ToListAsync());
        }

        // GET: DeskCounters/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deskCounter = await _context.DeskCounters
                .Include(d => d.QMSUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deskCounter == null)
            {
                return NotFound();
            }

            return View(deskCounter);
        }

        // GET: DeskCounters/Create
        public IActionResult Create()
        {
            ViewData["QMSUserId"] = new SelectList(_context.QMSUsers.Where(c => c.Email != "admin@nyatiqms.com"), "Id", "FullName");
            return View();
        }

        // POST: DeskCounters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CounterNumber,Status,QMSUserId")] DeskCounter deskCounter)
        {
            if (ModelState.IsValid)
            {
                Guid gg = Guid.NewGuid();
                var getuniqidd = gg;
                deskCounter.Id = getuniqidd.ToString();
                deskCounter.Status = true;
                _context.Add(deskCounter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            ViewData["QMSUserId"] = new SelectList(_context.QMSUsers, "Id", "Id", deskCounter.QMSUserId);
            return View(deskCounter);
        }

        // GET: DeskCounters/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deskCounter = await _context.DeskCounters.FindAsync(id);
            if (deskCounter == null)
            {
                return NotFound();
            }
            ViewData["QMSUserId"] = new SelectList(_context.QMSUsers.Where(c => c.Email != "admin@nyatiqms.com"), "Id", "FullName", deskCounter.QMSUserId);
            return View(deskCounter);
        }

        // POST: DeskCounters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,CounterNumber,Status,QMSUserId")] DeskCounter deskCounter)
        {
            if (id != deskCounter.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                
                try
                {
                    deskCounter.Status = true;
                    _context.Update(deskCounter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeskCounterExists(deskCounter.Id))
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
            ViewData["QMSUserId"] = new SelectList(_context.QMSUsers, "Id", "Id", deskCounter.QMSUserId);
            return View(deskCounter);
        }

        // GET: DeskCounters/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deskCounter = await _context.DeskCounters
                .Include(d => d.QMSUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deskCounter == null)
            {
                return NotFound();
            }

            return View(deskCounter);
        }

        // POST: DeskCounters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var deskCounter = await _context.DeskCounters.FindAsync(id);
            _context.DeskCounters.Remove(deskCounter);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeskCounterExists(string id)
        {
            return _context.DeskCounters.Any(e => e.Id == id);
        }
    }
}
