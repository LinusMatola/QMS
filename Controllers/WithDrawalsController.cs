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
using System.Security.Claims;
using Newtonsoft.Json.Linq;

namespace HubnyxQMS.Controllers
{
    [Authorize]
    public class WithDrawalsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WithDrawalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WithDrawals
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.WithDrawals.Include(w => w.QMSUser).Include(w => w.WithDrawalReason);
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> Reasons()
        {
            var applicationDbContext = _context.WithDrawalReasons;
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: WithDrawals/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var withDrawal = await _context.WithDrawals
                .Include(w => w.QMSUser)
                .Include(w => w.WithDrawalReason)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (withDrawal == null)
            {
                return NotFound();
            }

            return View(withDrawal);
        }

        // GET: WithDrawals/Create
        public IActionResult Create()
        {
            ViewData["QMSUserId"] = new SelectList(_context.QMSUsers, "Id", "Id");
            ViewData["WithDrawalReasonId"] = new SelectList(_context.WithDrawalReasons, "Id", "Name");
            return View();
        }
        public IActionResult CreateReason()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Addreason(string reason, WithDrawalReason withDrawalReason)
        {
            Guid gg = Guid.NewGuid();
            var getuniqidd = gg;
            withDrawalReason.Id = getuniqidd.ToString();
            withDrawalReason.Name = reason;
            _context.Add(withDrawalReason);
                await _context.SaveChangesAsync();
            return RedirectToAction("reasons", "withdrawals");
        }
        // POST: WithDrawals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string id,[Bind("TicketNumber,PhoneNumber,MemberReason,MemberNumber,WithDrawalReasonId,Status,Chat,QMSUserId")] WithDrawal withDrawal)
        {
            if (ModelState.IsValid)
            {
                //get phonenumber 

                string result = id.Substring(0, 1);
                var sectionid = "";
                if (result == "B")
                {
                    var getbosa = await _context.Sections.FirstOrDefaultAsync(c => c.Name == "BOSA");
                    sectionid = getbosa.Id;
                }
                else
                {
                    var getfosa = await _context.Sections.FirstOrDefaultAsync(c => c.Name == "FOSA");
                    sectionid = getfosa.Id;
                }

                var today = DateTime.UtcNow.AddHours(3).ToString("dd/MM/yyyy");
                var leftmemData = _context.Tickets.Where(c => (c.Created == today && c.SectionId == sectionid)).FirstOrDefault();

                var phonenumberleftmen = "";
                var leftJson = new JObject();
                //var myJson8 = new JObject();

                if (!string.IsNullOrEmpty(leftmemData.TicketReports))
                {
                    var myJson1 = JObject.Parse(leftmemData.TicketReports);
                    foreach (var jname in myJson1)
                    {
                        leftJson.Add(jname.Key, jname.Value);
                        if (jname.Key == id)
                        {
                            phonenumberleftmen = jname.Value.Value<dynamic>("PhoneNumber");
                            break;
                        }
                    }
                }



                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                Guid gg = Guid.NewGuid();
                var getuniqidd = gg;
                withDrawal.Id = getuniqidd.ToString();
                withDrawal.Created = DateTime.UtcNow.AddHours(3).ToString("dd/MM/yyyy HH:mm");
                withDrawal.TicketNumber = id;
                withDrawal.Status = false;
                withDrawal.PhoneNumber = phonenumberleftmen;
                withDrawal.QMSUserId = userId;
                _context.Add(withDrawal);
                await _context.SaveChangesAsync();


                //remove servicename
                var update = await _context.QMSUsers.FindAsync(userId);
                update.ServiceName = null;
                _context.Update(update);
                await _context.SaveChangesAsync();
                return RedirectToAction("index", "Home");
            }
            ViewData["QMSUserId"] = new SelectList(_context.QMSUsers, "Id", "Id", withDrawal.QMSUserId);
            ViewData["WithDrawalReasonId"] = new SelectList(_context.WithDrawalReasons, "Id", "Id", withDrawal.WithDrawalReasonId);
            return View(withDrawal);
        }

        // GET: WithDrawals/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var withDrawal = await _context.WithDrawals.FindAsync(id);
            if (withDrawal == null)
            {
                return NotFound();
            }
            ViewData["QMSUserId"] = new SelectList(_context.QMSUsers, "Id", "Id", withDrawal.QMSUserId);
            ViewData["WithDrawalReasonId"] = new SelectList(_context.WithDrawalReasons, "Id", "Id", withDrawal.WithDrawalReasonId);
            return View(withDrawal);
        }

        // POST: WithDrawals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Created,TicketNumber,PhoneNumber,MemberReason,MemberNumber,WithDrawalReasonId,Status,Chat,QMSUserId")] WithDrawal withDrawal)
        {
            if (id != withDrawal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(withDrawal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WithDrawalExists(withDrawal.Id))
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
            ViewData["QMSUserId"] = new SelectList(_context.QMSUsers, "Id", "Id", withDrawal.QMSUserId);
            ViewData["WithDrawalReasonId"] = new SelectList(_context.WithDrawalReasons, "Id", "Id", withDrawal.WithDrawalReasonId);
            return View(withDrawal);
        }

        // GET: WithDrawals/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var withDrawal = await _context.WithDrawals
                .Include(w => w.QMSUser)
                .Include(w => w.WithDrawalReason)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (withDrawal == null)
            {
                return NotFound();
            }

            return View(withDrawal);
        }

        // POST: WithDrawals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var withDrawal = await _context.WithDrawals.FindAsync(id);
            _context.WithDrawals.Remove(withDrawal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WithDrawalExists(string id)
        {
            return _context.WithDrawals.Any(e => e.Id == id);
        }
    }
}
