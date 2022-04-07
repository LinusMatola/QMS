using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HubnyxQMS.Data;
using HubnyxQMS.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using System.Management;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.IO;

namespace HubnyxQMS.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<SignalrServer> _signalrHub;
        private readonly IMemoryCache _memoryCache;

        public TicketsController(ApplicationDbContext context, IHubContext<SignalrServer> signalrHub, IMemoryCache memoryCache)
        {
            _context = context;
            _signalrHub = signalrHub;
            _memoryCache = memoryCache;
        }

        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<string> chekingpay(string phoneNumber, string serviceCode, string sessionId)
        //{
        //    var safRequest = Request;
        //    var MpesaResponse = await new StreamReader(safRequest.Body).ReadToEndAsync();
        //    try
        //    {
        //        return "Con What you want \n 1. choose this \n 2. choose that";
        //    }
        //    catch (Exception ex)
        //    {
                
        //    }


        //    return "";
        //}

        [Authorize]
        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tickets.ToListAsync());
        }

        public async Task<IActionResult> Screen()
        {
            return View();
        }
        [HttpGet]
        [Route("tickets/rate/{id}")]
        public async Task<IActionResult> Rate(string id, string S)
        {
            var T = id;
            if (T == null || S == null)
            {
                return NotFound();
            }
            var today = DateTime.Now.ToString("dd/MM/yyyy");
            string result = T.Substring(0, 1);
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
            var leftmemData = _context.Tickets.Where(c => (c.Created == today && c.SectionId == sectionid)).FirstOrDefault();

            var leftJson = new JObject();
            var secret = "";
            var lefttime = "";
            bool correctlink = false;
            if (!string.IsNullOrEmpty(leftmemData.TicketReports))
            {
                var myJson1 = JObject.Parse(leftmemData.TicketReports);
                foreach (var jname in myJson1)
                {
                    leftJson.Add(jname.Key, jname.Value);
                    if (jname.Key == T)
                    {
                        secret = jname.Value.Value<string>("SecretCode");
                        lefttime = jname.Value.Value<string>("LeftTime");
                        if (secret == S)
                        {
                            correctlink = true;
                            break;
                        }

                    }
                }
            }
            if(correctlink == true)
            {
                ViewBag.sec = S;
                ViewBag.Tic = T;
                ViewBag.left = lefttime;
                ViewBag.secid = sectionid;
                return View();
            }
            else
            {
                return Redirect("https://www.nyatisacco.co.ke/");
            }
            
        }
        [HttpPost]
        public async Task<IActionResult> Rating(string T, string S, string memrate, string lefttime, string sectionid)
        {
            
                //update the rating
                //check if expired
                var today = DateTime.UtcNow.AddHours(3).ToString("dd/MM/yyyy"); 
            var ttime = DateTime.UtcNow.AddHours(3).ToString("HH:mm");
                var memlefttime = Convert.ToDateTime(lefttime);
                var currenttime = Convert.ToDateTime(ttime);
                var diff = (currenttime - memlefttime).ToString();
                string result = diff.Substring(0, 5);
                string tores = result.Substring(result.Length - 2);
            int difff = Int32.Parse(tores);
            if (difff < 11)  
                {
                    //can rate
                    var ticketSample = JObject.Parse(_context.Tickets.Where(x => (x.Created == today && x.SectionId == sectionid)).ToListAsync().Result.First().TicketReports);
                    ticketSample[T]["Rate"] = memrate;
                    ticketSample[T]["Rated"] = "1";
                    var gettickt = await _context.Tickets.FirstOrDefaultAsync(x => (x.Created == today && x.SectionId == sectionid));
                    var updateticket = await _context.Tickets.FindAsync(gettickt.Id);
                    updateticket.TicketReports = ticketSample.ToString();
                    
                    _context.Update(updateticket);
                    await _context.SaveChangesAsync();

                var servedby = ticketSample[T]["ServedBy"].ToString();
                //update rating today user
                var getuser = await _context.QMSUsers.FirstOrDefaultAsync(c => c.FullName == servedby);
                var updaterate = await _context.QMSUsers.FindAsync(getuser.Id);
                int memberrate = Int32.Parse(memrate);
                int differennce = (5 - memberrate);
                updaterate.RatingToday = (updaterate.RatingToday - differennce);
                updaterate.RatingAllTime = (updaterate.RatingAllTime - differennce);
                _context.Update(updaterate);
                await _context.SaveChangesAsync();
                //update rating alltime user

                //update rating today tickets both

                //update rating service and section all time and today

            }

            return Redirect("https://www.nyatisacco.co.ke/");
        }
        [HttpGet]
        public IActionResult GetLoadScreen()
        {

            var res = _context.Screens
                .Include(c => c.DeskCounter)
                .ToList();
            return Ok(res);
        }
        [HttpGet]
        public IActionResult GetLoadScreenLower()
        {

            var res = _context.Screens
                .Include(c => c.DeskCounter)
                .ToList();
            return Ok(res);
        }
        public async Task<IActionResult> EditScreen()
        {
            ViewBag.active = await _context.Screens
                .Include(c => c.DeskCounter)
                .ToListAsync();
            return View();
        }
        public async Task<IActionResult> AddCounter()
        {
            ViewData["DeskCounterId"] = new SelectList(_context.DeskCounters, "Id", "CounterNumber");

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CounterAdd(string counternum)
        {
            var chekexist = await _context.Screens.FirstOrDefaultAsync(c => c.DeskCounterId == counternum);
            if(chekexist != null)
            {
                return RedirectToAction("EditScreen", "Tickets");
            }
            var checkcount = await _context.Screens.CountAsync();
            if(checkcount < 5)
            {
                var getnam = await _context.DeskCounters.FirstOrDefaultAsync(c => c.Id == counternum);
                Screen screen = new Screen();
                Guid gg = Guid.NewGuid();
                var getuniqidd = gg;
                screen.Id = getuniqidd.ToString();
                screen.DeskCounterId = counternum;
                screen.Created = DateTime.Now.ToString("dd/MM/yyyy");
                screen.DeskName = getnam.CounterNumber;
                screen.TicketNumber = "None";
                screen.Status = "Free";
                _context.Screens.Add(screen);
                await _context.SaveChangesAsync();
            }

            await _signalrHub.Clients.All.SendAsync("LoadScreen");
            await _signalrHub.Clients.All.SendAsync("LoadScreenLower");
            return RedirectToAction("EditScreen", "Tickets");
        }
        [HttpPost]
        public async Task<IActionResult> RemoveCounter(string id)
        {
            var rem = await _context.Screens.FindAsync(id);
            _context.Screens.Remove(rem);
            await _context.SaveChangesAsync();

            await _signalrHub.Clients.All.SendAsync("LoadScreen");
            await _signalrHub.Clients.All.SendAsync("LoadScreenLower");
            return RedirectToAction("EditScreen", "Tickets");
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public async Task<IActionResult> Create()
        {
            List<Section> HomePageDepartments;
            if (!_memoryCache.TryGetValue("Sections", out HomePageDepartments))
            {
                _memoryCache.Set("Departments", await _context.Sections.ToListAsync());
            }

            HomePageDepartments = _memoryCache.Get("Sections") as List<Section>;
            ViewBag.Departments = HomePageDepartments;

            ViewData["SectionId"] = new SelectList(_context.Set<Section>(), "Id", "Name");
            ViewBag.Section = await _context.Sections.ToListAsync();
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> AllServices(string id)
        {
            var DeptmntId = id;
            List<Service> Services;
            if (!_memoryCache.TryGetValue("Loc" + DeptmntId, out Services))
            {
                _memoryCache.Set("Loc" + DeptmntId, await _context.Services.Where(c => c.SectionId == DeptmntId).ToListAsync());
            }

            Services = _memoryCache.Get("Loc" + DeptmntId) as List<Service>;
            var servicelist = new SelectList(Services, "Id", "Name");
            return Json(servicelist);
        }
        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string sectionid, string servicenam, string phone,[Bind("TicketReports,RegisteredDevice,Created,ServedMembers,NoTurnOutMembers,Ratings,TotalEscalated,AverageWaitingTime,AverageServingTime,MembersBeingServed")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                
                //get machine Id
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                string MacAddress = String.Empty;

                foreach (ManagementObject mo in moc)
                {
                    if (MacAddress == String.Empty) // only return Mac address from first card
                    {
                        if ((bool)mo["IPEnabled"] == true) MacAddress = mo["MacAddress"].ToString();
                    }
                    mo.Dispose();
                }
                MacAddress = MacAddress.Replace(":", "-");

                var alloweddevice = "";

                var chars = "ABCDEFG";
                var stringChars = new char[3];
                var random = new Random();

                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }
                var secretcode = new String(stringChars);

                var currenttime = DateTime.UtcNow.AddHours(3).ToString("HH:mm");
                var currentdate = DateTime.Now.ToString("dd/MM/yyyy");

                var checkiftodayexists = await _context.Tickets.FirstOrDefaultAsync(c => (c.Created == currentdate && c.SectionId == sectionid));

                var getservice = await _context.Services
                    .Include(c => c.Section)
                    .FirstOrDefaultAsync(c => c.Id == servicenam);

                var ticketnum = "";
                if (checkiftodayexists == null)
                {
                    //delete all todays ticket
                    var selecttodays = await _context.TodaysTickets.Where(c => (c.Created != currentdate && c.SectionId == sectionid)).ToListAsync();
                    foreach(var del in selecttodays)
                    {
                        var rem = await _context.TodaysTickets.FindAsync(del.Id);
                        _context.TodaysTickets.Remove(rem);
                        await _context.SaveChangesAsync();
                    }

                    //check dep
                    var getsec = await _context.Sections.FirstOrDefaultAsync(c => c.Id == sectionid);
                    var s = getsec.Name.Substring(0, 1);
                    var number = "001";
                    var initializer = s + number;


                    //this is a new ticket
                    Guid gg = Guid.NewGuid();
                    var getuniqidd = gg;
                    ticket.Id = getuniqidd.ToString();
                    ticket.RegisteredDevice = MacAddress;
                    ticket.Created = currentdate;
                    ticket.ServedMembers = 0;
                    ticket.WaitingMembers = 1;
                    ticket.TotalMembers = 1;
                    ticket.NoTurnOutMembers = 0;
                    ticket.Ratings = 5;
                    ticket.TotalEscalated = 0;
                    ticket.AverageWaitingTime = "0 Minutes";
                    ticket.AverageServingTime = "0 Minutes";
                    ticket.MembersBeingServed = 0;
                    ticket.Closed = false;
                    ticket.SectionId = sectionid;
                    ticket.TicketReports = "{ '" + initializer + "':" + "{ 'CallTime': '"
                    + "0" + "','LeftTime': '"
                    + "0" + "','PhoneNumber': '"
                    + phone + "','Created': '"
                    + currenttime + "','Status': '"
                    + "0" + "','Served': '"
                    + "0" + "','TurnOut': '"
                    + "0" + "','Rate': '"
                    + "5" + "','Rated': '"
                    + "0" + "','Escalated': '"
                    + "0" + "','EscalatedTo': '"
                    + "0" + "','WaitingTime': '"
                    + "0" + "','ServedTime': '"
                    + "0" + "','ServedBy': '"
                    + "0" + "','Serviceid': '"
                    + getservice.Id + "','Section': '"
                    + getservice.Section.Name + "','Service': '"
                    + getservice.Name + "','NowServing': '"
                    + "0" + "','SecretCode': '"
                    + secretcode + "','Closed': '"
                    + "0" + "'} }";

                    _context.Add(ticket);
                    await _context.SaveChangesAsync();

                    ticketnum = initializer;
                    //deduct sms balance

                    var smsJsonPost = new JObject
                                            {
                                                { "api_key", "5fae8de671e67" },
                                                { "username", "maskani" },
                                                { "sender_id", "22136" },
                                                { "message", "Dear Member, Your Ticket No "+initializer+" ,Section "+getsec.Name+", Service "+getservice.Name+" will be served by our next available Agent, Nyati Sacco For you. Secret Code "+secretcode+"" },
                                                { "phone", phone}
                                            };

                    var smsData = smsJsonPost;
                    var smsEndPointUrl = "http://bulksms.mobitechtechnologies.com/api/sendsms";
                    var smsResponse = SendPostRequest(smsData, smsEndPointUrl);
                }
                else
                {
                    //add the new ticket
                    var tickett = await _context.Tickets.FindAsync(checkiftodayexists.Id);
                    var ticketJson = new JObject
                                    {
                                        { "CallTime", "0" },
                                        { "LeftTime", "0" },
                                        { "PhoneNumber", phone },
                                        { "Created", currenttime },
                                        { "Status", "0" },
                                        { "Served", "0" },
                                        { "TurnOut", "0" },
                                        { "Rate", "5" },
                                        { "Rated", "0" },
                                        { "Escalated", "0" },
                                        { "EscalatedTo", "0" },
                                        { "WaitingTime", "0" },
                                        { "ServedTime", "0" },
                                        { "ServedBy", "0" },
                                        { "Serviceid", getservice.Id },
                                        { "Section", getservice.Section.Name },
                                        { "Service", getservice.Name },
                                        { "NowServing", "0" },
                                        { "SecretCode", secretcode },
                                        { "Closed", "0" }
                                    };

                    var jsonData3 = JObject.Parse(tickett.TicketReports);

                    var getsec = await _context.Sections.FirstOrDefaultAsync(c => c.Id == sectionid);
                    var s = getsec.Name.Substring(0, 1);
                    var getticketno = ""+s+"" + ((tickett.TotalMembers + 1).ToString().PadLeft(3, '0'));

                    jsonData3.Add(getticketno, ticketJson);
                    tickett.TicketReports = jsonData3.ToString();

                    tickett.WaitingMembers = (tickett.WaitingMembers + 1);
                    tickett.TotalMembers = (tickett.TotalMembers + 1);
                    _context.Tickets.Update(tickett);
                    await _context.SaveChangesAsync();

                    ticketnum = getticketno;
                    //deduct sms balance

                    var smsJsonPost = new JObject
                                            {
                                                { "api_key", "5fae8de671e67" },
                                                { "username", "maskani" },
                                                { "sender_id", "22136" },
                                                { "message", "Dear Member, Your Ticket No "+getticketno+" ,Section "+getsec.Name+", Service "+getservice.Name+" will be served by our next available Agent, Nyati Sacco For you. Secret Code "+secretcode+"" },
                                                { "phone", phone}
                                            };

                    var smsData = smsJsonPost;
                    var smsEndPointUrl = "http://bulksms.mobitechtechnologies.com/api/sendsms";
                    var smsResponse = SendPostRequest(smsData, smsEndPointUrl);
                }
                var count = await _context.TodaysTickets.CountAsync();

                //we update ticket
                TodaysTicket todaysTicket = new TodaysTicket();
                Guid ggg = Guid.NewGuid();
                var getuniqiddd = ggg;
                todaysTicket.Id = getuniqiddd.ToString();
                todaysTicket.TicketNumber = ticketnum;
                todaysTicket.Created = currentdate;
                todaysTicket.CheckInTime = currenttime;
                todaysTicket.Served = false;
                todaysTicket.OrderNumber = (count + 1);
                todaysTicket.ServiceId = servicenam;
                todaysTicket.SectionId = getservice.Section.Id;

                _context.TodaysTickets.Add(todaysTicket);
                await _context.SaveChangesAsync();

                var checkreport = await _context.Reports.FirstOrDefaultAsync(c => c.Created == currentdate);
                if (checkreport == null)
                {
                    Guid gg = Guid.NewGuid();
                    var getuniqidd = gg;
                    //create report
                    Report report = new Report();
                    report.Id = getuniqidd.ToString();
                    report.Created = currentdate;
                    report.Date = DateTime.UtcNow.AddHours(3);
                    report.TotalMembers = 0;
                    report.TotalServed = 0;
                    report.AverageWaitingTime = 0;
                    report.TotalWaitingTime = 0;
                    report.AverageServingTime = 0;
                    report.TotalServingTime = 0;
                    report.Rating = 0;
                    report.EscalatedCases = 0;
                    report.EscalatedCasesSolved = 0;
                    report.EscalatedCasesAverage = 0;
                    _context.Reports.Add(report);
                    await _context.SaveChangesAsync();


                    //create service
                    var getallservices = await _context.Services.ToListAsync();
                    foreach (var item in getallservices)
                    {
                        ServicesReport servicesReport = new ServicesReport();
                        Guid ggser = Guid.NewGuid();
                        var getuniqiddser = ggser;
                        servicesReport.Id = getuniqiddser.ToString();
                        servicesReport.Created = currentdate;
                        servicesReport.Date = DateTime.UtcNow.AddHours(3);
                        servicesReport.TotalMembers = 0;
                        servicesReport.TotalServed = 0;
                        servicesReport.AverageWaitingTime = 0;
                        servicesReport.TotalWaitingTime = 0;
                        servicesReport.AverageServingTime = 0;
                        servicesReport.TotalServingTime = 0;
                        servicesReport.Rating = 0;
                        servicesReport.EscalatedCases = 0;
                        servicesReport.EscalatedCasesSolved = 0;
                        servicesReport.EscalatedCasesAverage = 0;
                        servicesReport.ServiceId = item.Id;
                        _context.ServicesReports.Add(servicesReport);
                        await _context.SaveChangesAsync();
                    }

                    // create section
                    var getallsections = await _context.Sections.ToListAsync();
                    foreach (var item in getallsections)
                    {
                        SectionReport sectionReport = new SectionReport();
                        Guid ggser = Guid.NewGuid();
                        var getuniqiddser = ggser;
                        sectionReport.Id = getuniqiddser.ToString();
                        sectionReport.Created = currentdate;
                        sectionReport.Date = DateTime.UtcNow.AddHours(3);
                        sectionReport.TotalMembers = 0;
                        sectionReport.TotalServed = 0;
                        sectionReport.AverageWaitingTime = 0;
                        sectionReport.TotalWaitingTime = 0;
                        sectionReport.AverageServingTime = 0;
                        sectionReport.TotalServingTime = 0;
                        sectionReport.Rating = 0;
                        sectionReport.EscalatedCases = 0;
                        sectionReport.EscalatedCasesSolved = 0;
                        sectionReport.EscalatedCasesAverage = 0;
                        sectionReport.SectionId = item.Id;
                        _context.SectionReports.Add(sectionReport);
                        await _context.SaveChangesAsync();
                    }
                }

                //update section
                var getsecc = await _context.SectionReports.FirstOrDefaultAsync(c => (c.SectionId == sectionid && c.Created == currentdate));
                getsecc.TotalMembers = (getsecc.TotalMembers + 1);
                _context.SectionReports.Update(getsecc);
                await _context.SaveChangesAsync();
                //update report
                var getrep = await _context.Reports.FirstOrDefaultAsync(c => c.Created == currentdate);
                getrep.TotalMembers = (getrep.TotalMembers + 1);
                _context.Reports.Update(getrep);
                await _context.SaveChangesAsync();
                //update service
                var getser = await _context.ServicesReports.FirstOrDefaultAsync(c => (c.ServiceId == getservice.Id && c.Created == currentdate));
                getser.TotalMembers = (getser.TotalMembers + 1);
                _context.ServicesReports.Update(getser);
                await _context.SaveChangesAsync();


                await _signalrHub.Clients.All.SendAsync("TotalMembers");
                await _signalrHub.Clients.All.SendAsync("LoadTickets");
                await _signalrHub.Clients.All.SendAsync("LoadScreen");
                await _signalrHub.Clients.All.SendAsync("LoadScreenLower");
                return RedirectToAction(nameof(Create));
            }
            return View(ticket);
        }
        [Authorize]
        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);
        }
        [Authorize]
        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,TicketReports,RegisteredDevice,Created,ServedMembers,NoTurnOutMembers,Ratings,TotalEscalated,AverageWaitingTime,AverageServingTime,MembersBeingServed")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
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
            return View(ticket);
        }
        [Authorize]
        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }
        [Authorize]
        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(string id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }
        public static async Task<string> SendPostRequest(JObject data, string url)
        {

            string myJson = data.ToString();
            string smsPostResponse = "";
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(
                    url,
                     new StringContent(myJson, Encoding.UTF8, "application/json"));
                smsPostResponse = response.StatusCode.ToString();
            }

            return smsPostResponse;
        }
    }
}
