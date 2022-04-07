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
    public class SectionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sections
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sections.ToListAsync());
        }

        // GET: Sections/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var section = await _context.Sections
                .FirstOrDefaultAsync(m => m.Id == id);
            if (section == null)
            {
                return NotFound();
            }

            return View(section);
        }

        // GET: Sections/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Section section)
        {
            if (ModelState.IsValid)
            {
                //check
                var check = await _context.Sections.FirstOrDefaultAsync(c => c.Name == section.Name);
                if(check != null)
                {
                    ViewBag.Error = "Name Exists";
                    return RedirectToAction(nameof(Index));
                }
                Guid gg = Guid.NewGuid();
                var getuniqidd = gg;
                section.Id = getuniqidd.ToString();
                section.Name = section.Name.ToUpper();
                _context.Add(section);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Reports");
            }
            return View(section);
        }

        // GET: Sections/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var section = await _context.Sections.FindAsync(id);
            if (section == null)
            {
                return NotFound();
            }
            return View(section);
        }

        // POST: Sections/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name")] Section section)
        {
            if (id != section.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(section);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SectionExists(section.Id))
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
            return View(section);
        }

        // GET: Sections/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var section = await _context.Sections
                .FirstOrDefaultAsync(m => m.Id == id);
            if (section == null)
            {
                return NotFound();
            }

            return View(section);
        }

        // POST: Sections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var section = await _context.Sections.FindAsync(id);
            _context.Sections.Remove(section);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SectionExists(string id)
        {
            return _context.Sections.Any(e => e.Id == id);
        }
    }
}
