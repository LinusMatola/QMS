using HubnyxQMS.Data;
using HubnyxQMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace HubnyxQMS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<QMSUser> _userManager;
        private readonly IHubContext<SignalrServer> _signalrHub;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<QMSUser> userManager, IHubContext<SignalrServer> signalrHub)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signalrHub = signalrHub;
        }

        public async Task<IActionResult> Index()
        {
            
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var getemail = await _context.QMSUsers.FirstOrDefaultAsync(c => c.Id == userId);
            var currentdate = DateTime.UtcNow.AddHours(3).ToString("dd/MM/yyyy");
            ViewBag.lastupdate = DateTime.UtcNow.AddHours(3).ToString("HH:mm");
            //check if today has been added to report
            var checkreport = await _context.Reports.FirstOrDefaultAsync(c => c.Created == currentdate);
            if(checkreport == null)
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
                foreach(var item in getallservices)
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
                //update user
                var allusers = await _context.QMSUsers.ToListAsync();
                foreach(var item in allusers)
                {
                    var update = await _context.QMSUsers.FindAsync(item.Id);
                    update.ServedToday = 0;
                    update.SuccessRateToday = 0;
                    update.RatingToday = 5;
                    update.NowServing = null;
                    update.NowServingSecret = null;
                    update.ServiceName = null;
                    _context.QMSUsers.Update(update);
                    await _context.SaveChangesAsync();
                }

                //update screen
                var screen = await _context.Screens.ToListAsync();
                foreach(var item in screen)
                {
                    var updatescreen = await _context.Screens.FindAsync(item.Id);
                    updatescreen.TicketNumber = "None";
                    updatescreen.Created = currentdate;
                    updatescreen.Status = "Free";
                    _context.Screens.Update(updatescreen);
                    await _context.SaveChangesAsync();
                }
            }

            //check if user can view dashboard
            if (getemail.CanViewDashboard == false)
            {
                return RedirectToAction("MyEscalate", "Home");
            }

            ViewBag.totalserved = getemail.ServedToday;
            ViewBag.nowserving = getemail.NowServing;
            ViewBag.secid = getemail.NowServingSecret;
            ViewBag.sectionid = getemail.NowServingSectionId;
            ViewBag.checkwithdrawal = getemail.ServiceName;

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Comms()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var getemail = await _context.QMSUsers.FirstOrDefaultAsync(c => c.Id == userId);
            //clear today data
            var users = await _context.InternalComms
                .Where(c => c.ReceiverEmail == getemail.Email)
                .OrderByDescending(c => c.Order)
                .ToListAsync();


            //update all as read on report

            var jsonData = _context.QMSUsers.Where(c => c.Id == userId).FirstOrDefault();

            var myJson = new JObject();
            //var myJson8 = new JObject();

            if (!string.IsNullOrEmpty(jsonData.Notifications))
            {
                var myJson1 = JObject.Parse(jsonData.Notifications);
                foreach (var jname in myJson1)
                {
                    myJson.Add(jname.Key, jname.Value);
                    var updateticket = JObject.Parse(_context.QMSUsers.Where(x => x.Id == userId).ToListAsync().Result.First().Notifications);
                    updateticket[jname.Key]["Read"] = "1"; 
                    var updateuser = await _context.QMSUsers.FindAsync(userId);
                    updateuser.Notifications = updateticket.ToString();
                    _context.Update(updateuser);
                    await _context.SaveChangesAsync();
                }
            }


            
            return View(users);
        }
        [HttpGet]
        public async Task<IActionResult> CreateInternalMessage()
        {
            ViewData["QMSUsersId"] = new SelectList(_context.QMSUsers, "Id", "FullName");

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> sendnow(string message, string staffid)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var getemail = await _context.QMSUsers.FirstOrDefaultAsync(c => c.Id == userId);
            var getrecemail = await _context.QMSUsers.FirstOrDefaultAsync(c => c.Id == staffid);

            var getall = await _context.InternalComms.CountAsync();

            InternalComm internalComm = new InternalComm();
            Guid gg = Guid.NewGuid();
            var getuniqidd = gg;
            internalComm.Id = getuniqidd.ToString();
            internalComm.Message = message;
            internalComm.SenderEmail = getemail.Email;
            internalComm.ReceiverEmail = getrecemail.Email;
            internalComm.Created = DateTime.UtcNow.AddHours(3).ToString("dd/MM/yyyy HH:mm");
            internalComm.Seen = false;
            internalComm.Order = (getall + 1);
            _context.Add(internalComm);
            await _context.SaveChangesAsync();

            string result = message.Substring(0, 3);
            result = ""+result+"................";
            var currenttime = DateTime.UtcNow.AddHours(3).ToString("dd/MM/yyyy HH:mm");
            //lets update message
            if (getrecemail.Notifications == null)
            {
                var upt = await _context.QMSUsers.FindAsync(getrecemail.Id);

                var initializer = getuniqidd.ToString();
                upt.Notifications = "{ '" + initializer + "':" + "{ 'Message': '"
                    + result + "','ProfilePic': '"
                    + getemail.ProfilePicture + "','Name': '"
                    + getemail.FullName + "','Time': '"
                    + currenttime + "','Read': '"
                    + "0" + "'} }";
                _context.QMSUsers.Update(upt);
                await _context.SaveChangesAsync();
            }
            else
            {
                var mychat = await _context.QMSUsers.FindAsync(getrecemail.Id);
                var chatJson = new JObject
                                    {
                                        { "Message", result },
                                        { "ProfilePic", getemail.ProfilePicture },
                                        { "Name", getemail.FullName },
                                        { "Time", currenttime },
                                        { "Read", "0" }
                                    };

                var jsonData3 = JObject.Parse(mychat.Notifications);

                jsonData3.Add(getuniqidd.ToString(), chatJson);
                mychat.Notifications = jsonData3.ToString();

                _context.QMSUsers.Update(mychat);
                await _context.SaveChangesAsync();
            }

            await _signalrHub.Clients.All.SendAsync("chat");
            return RedirectToAction("Comms", "Home");

        }
        public async Task<IActionResult> MyServices()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var getemail = await _context.QMSUsers.FirstOrDefaultAsync(c => c.Id == userId);
            //clear today data
            var users = await _context.QMSUserService
                .Include(c => c.Service)
                .ThenInclude(c => c.Section)
                .Where(c => c.QMSUserId == userId)
                .ToListAsync();
            return View(users);
        }
        [HttpGet]
        public async Task<IActionResult> ReplyToMember(string id)
        {
            var getticketnumber = await _context.Escalates.FirstOrDefaultAsync(c => c.Id == id);
            ViewBag.tictet = getticketnumber.TicketNumber;
            ViewBag.id = id;
           return View();
        }
        [HttpPost]
        public async Task<IActionResult> SendTheMessage(string id, string phone, string mymessage)
        {
            var update = await _context.Escalates.FindAsync(id);
            update.ReplytoMember = mymessage;
            _context.Escalates.Update(update);
            await _context.SaveChangesAsync();

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var getemail = await _context.QMSUsers.FirstOrDefaultAsync(c => c.Id == userId);

            var smsJsonPost1 = new JObject
                                            {
                                                { "api_key", "5fae8de671e67" },
                                                { "username", "maskani" },
                                                { "sender_id", "22136" },
                                                { "message", ""+mymessage+" sent by: "+getemail.FullName+""},
                                                { "phone", phone}
                                            };

            var smsData1 = smsJsonPost1;
            var smsEndPointUrl1 = "http://bulksms.mobitechtechnologies.com/api/sendsms";
            var smsResponse1 = SendPostRequest1(smsData1, smsEndPointUrl1);


            return RedirectToAction("ReplyToMember", "Home", new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> closecase(string id)
        {
            var update = await _context.Escalates.FindAsync(id);
            update.Status = true;
            _context.Escalates.Update(update);
            await _context.SaveChangesAsync();
            return RedirectToAction("Myescalate", "Home", new { id = id });
        }
        [HttpPost]
        public async Task<IActionResult> opencase(string id)
        {
            var update = await _context.Escalates.FindAsync(id);
            update.Status = false;
            _context.Escalates.Update(update);
            await _context.SaveChangesAsync();
            return RedirectToAction("Myescalate", "Home", new { id = id });
        }
        [HttpGet]
        public async Task<IActionResult> escalate(string id)
        {
            //get phonenumber
            

            ViewBag.id = id;
            ViewData["QMSUsersId"] = new SelectList(_context.QMSUsers.Where(c => c.CanViewDashboard == false), "Id", "FullName");
            return View();
        }
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> AllEscalated(string id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var getemail = await _context.QMSUsers.FirstOrDefaultAsync(c => c.Id == userId);
            ViewBag.email = getemail.Email;
            var escaltes = await _context.Escalates.ToListAsync();
            return View(escaltes);
        }
        public async Task<IActionResult> ReplyToEscalated(string id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var getemail = await _context.QMSUsers.FirstOrDefaultAsync(c => c.Id == userId);
            ViewBag.email = getemail.Email;
            ViewBag.propic = getemail.ProfilePicture;
            ViewBag.id = id;
            var escaltes = await _context.Escalates.ToListAsync();

            var jsonData = _context.Escalates.Where(c => c.Id == id).FirstOrDefault();

            var myJson = new JObject();
            //var myJson8 = new JObject();

            if (!string.IsNullOrEmpty(jsonData.Chat))
            {
                var myJson1 = JObject.Parse(jsonData.Chat);
                foreach (var jname in myJson1)
                {
                    myJson.Add(jname.Key, jname.Value);
                }
            }
            ViewBag.messages = myJson;

            return View(escaltes);
        }
        [HttpPost]
        public async Task<IActionResult> Chatreply(string id, string message)
        {
            //check if is null
            var currenttime = DateTime.UtcNow.AddHours(3).ToString("MMMM dd, yyyy HH:mm");

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var getemail = await _context.QMSUsers.FirstOrDefaultAsync(c => c.Id == userId);
            var check = await _context.Escalates.FirstOrDefaultAsync(c => c.Id == id);
            Guid gg = Guid.NewGuid();
            var getuniqidd = gg;
            if (check.Chat == null)
            {
                var upt = await _context.Escalates.FindAsync(id);

                var initializer = getuniqidd.ToString();
                upt.Chat = "{ '" + initializer + "':" + "{ 'Message': '"
                    + message + "','ProfilePic': '"
                    + getemail.ProfilePicture + "','Time': '"
                    + currenttime + "','Email': '"
                    + getemail.Email + "'} }";
                upt.TotalChats = (upt.TotalChats + 1);
                _context.Escalates.Update(upt);
                await _context.SaveChangesAsync();
            }
            else
            {
                var mychat = await _context.Escalates.FindAsync(id);
                var chatJson = new JObject
                                    {
                                        { "Message", message },
                                        { "ProfilePic", getemail.ProfilePicture },
                                        { "Time", currenttime },
                                        { "Email", getemail.Email }
                                    };

                var jsonData3 = JObject.Parse(mychat.Chat);


                jsonData3.Add(getuniqidd.ToString(), chatJson);
                mychat.Chat = jsonData3.ToString();

                mychat.TotalChats = (mychat.TotalChats + 1);
                _context.Escalates.Update(mychat);
                await _context.SaveChangesAsync();
            }

            var result = "";
            //get receiver
            if (userId == check.QMSUserId)
            {
                var receiver = await _context.QMSUsers.FirstOrDefaultAsync(c => c.Id == userId);
                result = "You have a chat reply from " + receiver.FullName + "";
                if (receiver.Notifications == null)
                {
                    var upt = await _context.QMSUsers.FindAsync(receiver.Id);

                    var initializer = getuniqidd.ToString();
                    upt.Notifications = "{ '" + initializer + "':" + "{ 'Message': '"
                        + result + "','ProfilePic': '"
                        + getemail.ProfilePicture + "','Name': '"
                        + getemail.FullName + "','Time': '"
                        + currenttime + "','Read': '"
                        + "0" + "'} }";
                    _context.QMSUsers.Update(upt);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var mychat = await _context.QMSUsers.FindAsync(receiver.Id);
                    var chatJson = new JObject
                                    {
                                        { "Message", result },
                                        { "ProfilePic", getemail.ProfilePicture },
                                        { "Name", getemail.FullName },
                                        { "Time", currenttime },
                                        { "Read", "0" }
                                    };

                    var jsonData3 = JObject.Parse(mychat.Notifications);


                    jsonData3.Add(getuniqidd.ToString(), chatJson);
                    mychat.Notifications = jsonData3.ToString();

                    _context.QMSUsers.Update(mychat);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                var receiver = await _context.QMSUsers.FirstOrDefaultAsync(c => c.Id == check.QMSUserId);
                result = "You have a chat reply from " + getemail.FullName + "";
                if (getemail.Notifications == null)
                {
                    var upt = await _context.QMSUsers.FindAsync(getemail.Id);

                    var initializer = getuniqidd.ToString();
                    upt.Notifications = "{ '" + initializer + "':" + "{ 'Message': '"
                        + result + "','ProfilePic': '"
                        + getemail.ProfilePicture + "','Name': '"
                        + getemail.FullName + "','Time': '"
                        + currenttime + "','Read': '"
                        + "0" + "'} }";
                    _context.QMSUsers.Update(upt);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var mychat = await _context.QMSUsers.FindAsync(getemail.Id);
                    var chatJson = new JObject
                                    {
                                        { "Message", result },
                                        { "ProfilePic", getemail.ProfilePicture },
                                        { "Name", getemail.FullName },
                                        { "Time", currenttime },
                                        { "Read", "0" }
                                    };

                    var jsonData3 = JObject.Parse(mychat.Notifications);


                    jsonData3.Add(getuniqidd.ToString(), chatJson);
                    mychat.Notifications = jsonData3.ToString();

                    _context.QMSUsers.Update(mychat);
                    await _context.SaveChangesAsync();
                }
            }
            
            


            return RedirectToAction("ReplyToEscalated", "Home", new { id = id});
        }
        [HttpPost]
        public async Task<IActionResult> escalatenow(string staffid, string reason,string currentticket)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var getemail = await _context.QMSUsers.FirstOrDefaultAsync(c => c.Id == userId);
            Escalate escalate = new Escalate();
            Guid gg = Guid.NewGuid();
            var getuniqidd = gg;
            escalate.Id = getuniqidd.ToString();
            escalate.Reason = reason;
            escalate.QMSUserId = staffid;
            escalate.TicketNumber = currentticket;
            escalate.Created = DateTime.Now.ToString("dd/MM/yyyy");
            escalate.CreatedTime = DateTime.UtcNow.AddHours(3).ToString("HH:mm");
            escalate.EscalatedBy = getemail.Email;
            escalate.Status = false;
            _context.Escalates.Add(escalate);
            await _context.SaveChangesAsync();





            return RedirectToAction("Escalated", "Home");
        }
        public async Task<IActionResult> Escalated()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var getemail = await _context.QMSUsers.FirstOrDefaultAsync(c => c.Id == userId);

            var checkifstaff = await _context.Escalates.FirstOrDefaultAsync(c => c.QMSUserId == userId);
            if(checkifstaff != null)
            {
                return RedirectToAction("MyEscalate", "Home");
            }

            var esc = await _context.Escalates
                .Include(c => c.QMSUser)
                .Where(c => c.EscalatedBy == getemail.Email)
                .OrderByDescending(c => c.Created)
                .ToListAsync();
            return View(esc);
        }
        public async Task<IActionResult> MyEscalate()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var checkifstaff = await _context.Escalates
                .Include(c => c.QMSUser)
                .Where(c => c.QMSUserId == userId)
                .OrderByDescending(c => c.Created)
                .ToListAsync();
            return View(checkifstaff);
        }
        public async Task<IActionResult> CallMember(string currentmember,string sectionid)
        {
            

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var getemail = await _context.QMSUsers.FirstOrDefaultAsync(c => c.Id == userId);
            var today = DateTime.Now.ToString("dd/MM/yyyy");
            var currenttime = DateTime.UtcNow.AddHours(3).ToString("HH:mm");

            //if user has no counter dont call
            var getdesk = await _context.DeskCounters.FirstOrDefaultAsync(c => c.QMSUserId == userId);
            if (getdesk == null)
            {
                //return home
                return RedirectToAction(nameof(Index));
            }


            //here update the current member
            if (currentmember != null)
            {
                //first check if is withdrawal 
                var checkwithdrawal = await _context.QMSUsers.FirstOrDefaultAsync(c => c.Id == userId);
                if (checkwithdrawal.ServiceName == "WITHDRAWAL")
                {
                    //return home
                    ViewBag.Error = "Update WithDrawal First";
                    return RedirectToAction(nameof(Index));
                }

                var leftmemData = _context.Tickets.Where(c => (c.Created == today && c.SectionId == sectionid)).FirstOrDefault();

                var phonenumberleftmen = "";
                var calltime = "";
                var waitingtime = "";
                var servingtime = "";
                var servingid = "";
                var secrett = "";
                var leftJson = new JObject();
                //var myJson8 = new JObject();

                if (!string.IsNullOrEmpty(leftmemData.TicketReports))
                {
                    var myJson1 = JObject.Parse(leftmemData.TicketReports);
                    foreach (var jname in myJson1)
                    {
                        leftJson.Add(jname.Key, jname.Value);
                        if (jname.Key == currentmember)
                        {
                            phonenumberleftmen = jname.Value.Value<dynamic>("PhoneNumber");
                            calltime = jname.Value.Value<dynamic>("CallTime");
                            waitingtime = jname.Value.Value<dynamic>("WaitingTime");
                            servingtime = jname.Value.Value<dynamic>("ServedTime");
                            servingid = jname.Value.Value<dynamic>("Serviceid");
                            secrett = jname.Value.Value<dynamic>("SecretCode");
                            break;
                        }
                    }
                }
                //updating current member information
                //update ticketfirst
                var ticketSample = JObject.Parse(_context.Tickets.Where(x => (x.Created == today && x.SectionId == sectionid)).ToListAsync().Result.First().TicketReports);
                ticketSample[currentmember]["LeftTime"] = currenttime;
                ticketSample[currentmember]["Status"] = "1";
                ticketSample[currentmember]["Served"] = "1";
                ticketSample[currentmember]["TurnOut"] = "1";
                ticketSample[currentmember]["ServedBy"] = getemail.FullName;
                ticketSample[currentmember]["NowServing"] = "0";
                ticketSample[currentmember]["Closed"] = "1";

                //calculate served time
                var membercalltime = Convert.ToDateTime(calltime);
                var membercurrenttime = Convert.ToDateTime(currenttime);
                var getdifference = (membercurrenttime - membercalltime);

                ticketSample[currentmember]["ServedTime"] = getdifference;

                var gettickt = await _context.Tickets.FirstOrDefaultAsync(x => (x.Created == today && x.SectionId == sectionid));
                var updateticket = await _context.Tickets.FindAsync(gettickt.Id);
                updateticket.TicketReports = ticketSample.ToString();

                _context.Update(updateticket);
                await _context.SaveChangesAsync();

                //update on the screen
                var getscreeen = await _context.Screens.FirstOrDefaultAsync(c => c.DeskCounterId == getdesk.Id);
                var updatethescreen = await _context.Screens.FindAsync(getscreeen.Id);
                updatethescreen.TicketNumber = "None";
                updatethescreen.Status = "Free";
                _context.Update(updatethescreen);
                await _context.SaveChangesAsync();

                //update report

                var getmin = waitingtime;
                string hour = getmin.Substring(0, 2);
                string min = getmin.Substring(3, 5);
                string truemin = min.Substring(0, 2);

                int hr = Int32.Parse(hour);
                int minutes = Int32.Parse(truemin);
                int totalwaiting = 0;

                if (hr > 0)
                {
                    int totalmin = (hr * 60);
                    totalwaiting = (totalmin + minutes);
                }
                else
                {
                    totalwaiting = minutes;
                }


                var getminser = getdifference.ToString();
                string hourser = getminser.Substring(0, 2);
                string minser = getminser.Substring(3, 5);
                string trueminser = minser.Substring(0, 2);

                int hrser = Int32.Parse(hourser);
                int minutesser = Int32.Parse(trueminser);
                int totalser = 0;

                if (hrser > 0)
                {
                    int totalminser = (hrser * 60);
                    totalser = (totalminser + minutesser);
                }
                else
                {
                    totalser = minutesser;
                }
                //update report
                var getrep = await _context.Reports.FirstOrDefaultAsync(c => c.Created == today);
                getrep.TotalServed = (getrep.TotalServed + 1);
                getrep.TotalWaitingTime = (getrep.TotalWaitingTime + totalwaiting);
                getrep.TotalServingTime = (getrep.TotalServingTime + totalser);
                _context.Reports.Update(getrep);
                await _context.SaveChangesAsync();


                //update section
                var getsecc = await _context.SectionReports.FirstOrDefaultAsync(c => (c.SectionId == sectionid && c.Created == today));
                getsecc.TotalServed = (getsecc.TotalServed + 1);
                getsecc.TotalWaitingTime = (getsecc.TotalWaitingTime + totalwaiting);
                getsecc.TotalServingTime = (getsecc.TotalServingTime + totalser);
                _context.SectionReports.Update(getsecc);
                await _context.SaveChangesAsync();


                //update service
                var getser = await _context.ServicesReports.FirstOrDefaultAsync(c => (c.ServiceId == servingid && c.Created == today));
                getser.TotalServed = (getser.TotalServed + 1);
                getser.TotalWaitingTime = (getser.TotalWaitingTime + totalwaiting);
                getser.TotalServingTime = (getser.TotalServingTime + totalser);
                _context.ServicesReports.Update(getser);
                await _context.SaveChangesAsync();

                //update qmsuserservice
                var getqmsu = await _context.QMSUserService.FirstOrDefaultAsync(c => (c.ServiceId == servingid && c.QMSUserId == userId));
                var uptqms = await _context.QMSUserService.FindAsync(getqmsu.Id);
                uptqms.ServedAllTime = (uptqms.ServedAllTime + 1);
                _context.QMSUserService.Update(uptqms);
                await _context.SaveChangesAsync();

                //update user
                var upddateuser = await _context.QMSUsers.FindAsync(userId);
                upddateuser.ServedAlltime = (upddateuser.ServedAlltime + 1);
                upddateuser.ServedToday = (upddateuser.ServedToday + 1);
                upddateuser.RatingAllTime = (upddateuser.RatingAllTime + 5);
                upddateuser.RatingToday = (upddateuser.RatingToday + 5);
                _context.QMSUsers.Update(upddateuser);
                await _context.SaveChangesAsync();

                //update ticket
                var updatetick = await _context.Tickets.FirstOrDefaultAsync(c => (c.SectionId == sectionid && c.Created == today));
                var upttic = await _context.Tickets.FindAsync(updatetick.Id);
                upttic.ServedMembers = (upttic.ServedMembers + 1);
                _context.Tickets.Update(upttic);
                await _context.SaveChangesAsync();

                //send to the one who left
                var smsJsonPost1 = new JObject
                                            {
                                                { "api_key", "5fae8de671e67" },
                                                { "username", "maskani" },
                                                { "sender_id", "22136" },
                                                { "message", "Thankyou for visiting Nyati Sacco, for self service *346#. You were served by "+getemail.FullName.ToUpper()+", rate the service on https://192.168.137.180:45456/Tickets/Rate/"+currentmember+"?S="+secrett+" expires in 10 minutes" },
                                                { "phone", phonenumberleftmen}
                                            };

                var smsData1 = smsJsonPost1;
                var smsEndPointUrl1 = "http://bulksms.mobitechtechnologies.com/api/sendsms";
                var smsResponse1 = SendPostRequest1(smsData1, smsEndPointUrl1);
            }










            //from here we register a new ticket




            var currentday = await _context.Tickets.Where(c => (c.Created == today && c.SectionId == sectionid)).FirstOrDefaultAsync();
            var jsonData = _context.Tickets.Where(c => (c.Created == today && c.SectionId == sectionid)).FirstOrDefault();


            

            var nxtticket = "";
            var phonenumber = "";
            var secret = "";
            bool exausted = true;
            var ticketserviceid = "";
            var newserviceid = "";
            var createdd = "";
            var myJson = new JObject();

            //call member who is on my service

            if (!string.IsNullOrEmpty(jsonData.TicketReports))
            {
                var myJson1 = JObject.Parse(jsonData.TicketReports);
                foreach (var jname in myJson1)
                {
                    myJson.Add(jname.Key, jname.Value);
                    if(jname.Value.Value<dynamic>("Status") == "0")
                    {
                        ticketserviceid = jname.Value.Value<string>("Serviceid");
                        //check if has permission
                        var checkifpermited = await _context.QMSUserService.FirstOrDefaultAsync(c => (c.QMSUserId == userId && c.ServiceId == ticketserviceid));
                        if(checkifpermited != null)
                        {
                            nxtticket = jname.Key;
                            phonenumber = jname.Value.Value<dynamic>("PhoneNumber");
                            secret = jname.Value.Value<dynamic>("SecretCode");
                            newserviceid = jname.Value.Value<dynamic>("Serviceid");
                            createdd = jname.Value.Value<dynamic>("Created");
                            exausted = false;
                            break;
                        }
                    }
                }
            }

            if (exausted == true)
            {
                ViewBag.Error = "All Members Served";
                return RedirectToAction(nameof(Index));
            }

            var passsectionid = await _context.Services
                .Include(c => c.Section)
                .FirstOrDefaultAsync(c => c.Id == newserviceid);
            ViewBag.sectionid = passsectionid.Section.Id;
            //call the next member
            var jsonSample = JObject.Parse(_context.Tickets.Where(x => x.Created == today).ToListAsync().Result.First().TicketReports);
            jsonSample[nxtticket]["CallTime"] = currenttime;
            jsonSample[nxtticket]["Status"] = "1";


            var membercreatedtime = Convert.ToDateTime(createdd);
            var membercurrentttime = Convert.ToDateTime(currenttime);
            var getdifferencewaiting = (membercurrentttime - membercreatedtime);


            jsonSample[nxtticket]["WaitingTime"] = getdifferencewaiting;
            jsonSample[nxtticket]["NowServing"] = "1";

            var assijgn = await _context.Tickets.FindAsync(currentday.Id);
            assijgn.WaitingMembers = (assijgn.WaitingMembers - 1);
            assijgn.TicketReports = jsonSample.ToString();

            _context.Update(assijgn);
            await _context.SaveChangesAsync();
            //updateuser

            var user= await _context.QMSUsers.FindAsync(userId);
            user.Created = today;
            user.NowServing = nxtticket;
            user.NowServingSecret = secret;
            user.NowServingSectionId = passsectionid.Section.Id;
            user.ServiceName = passsectionid.Name;
            //user.RatingToday = nxtticket;
            //user.AverageServingTime = "to be calculated";
            _context.Update(user);
            await _context.SaveChangesAsync();

            //update ticket
            var updatetodaysticket = await _context.TodaysTickets.FirstOrDefaultAsync(c => c.TicketNumber == nxtticket);
            var updateTT = await _context.TodaysTickets.FindAsync(updatetodaysticket.Id);
            updateTT.Served = true;
            updateTT.Serving = true;
            _context.Update(updateTT);
            await _context.SaveChangesAsync();

            //update qmsuserservice
            //var updateqmsservice = await _context.QMSUserService.FirstOrDefaultAsync(c => (c.QMSUserId == userId && c.ServiceId == ticketserviceid));
            //var updateqmserv = await _context.QMSUserService.FindAsync(updateqmsservice.Id);
            //updateqmserv.ServedAllTime = (updateqmserv.ServedAllTime + 1);
            //updateqmserv.ServedToday = (updateqmserv.ServedToday + 1);
            //_context.Update(updateqmserv);
            //await _context.SaveChangesAsync();


            //update screen
            var getscrn = await _context.Screens.FirstOrDefaultAsync(c => c.DeskCounterId == getdesk.Id);
            var updatescreen = await _context.Screens.FindAsync(getscrn.Id);
            updatescreen.TicketNumber = nxtticket;
            updatescreen.Status = "Engaged";
            _context.Update(updatescreen);
            await _context.SaveChangesAsync();

            


            await _signalrHub.Clients.All.SendAsync("TotalMembers");
            await _signalrHub.Clients.All.SendAsync("LoadTickets");
            await _signalrHub.Clients.All.SendAsync("LoadScreen");
            await _signalrHub.Clients.All.SendAsync("LoadScreenLower");

            SpeechSynthesizer _SS = new SpeechSynthesizer();
            _SS.Volume = 100;
            _SS.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Teen);
            _SS.Rate = -2;
            _SS.SetOutputToDefaultAudioDevice();
            _SS.Speak("Ticket Number "+ nxtticket + ", Please proceed to Counter Number " + getdesk.CounterNumber + ".");


            //send sms to ask him in
            var smsJsonPost = new JObject
                                            {
                                                { "api_key", "5fae8de671e67" },
                                                { "username", "maskani" },
                                                { "sender_id", "22136" },
                                                { "message", "Dear Member, Thankyou for waiting, Our Agent Counter "+getdesk.CounterNumber+" is now ready to serve you, Nyati Sacco for you" },
                                                { "phone", phonenumber}
                                            };

            var smsData = smsJsonPost;
            var smsEndPointUrl = "http://bulksms.mobitechtechnologies.com/api/sendsms";
            var smsResponse = SendPostRequest(smsData, smsEndPointUrl);



            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Close(string currentmember, string sectionid)
        {
            var today = DateTime.Now.ToString("dd/MM/yyyy");
            var currenttime = DateTime.UtcNow.AddHours(3).ToString("HH:mm");
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentday = await _context.Tickets.Where(c => (c.Created == today && c.SectionId == sectionid)).FirstOrDefaultAsync();
            var jsonData = _context.Tickets.Where(c => (c.Created == today && c.SectionId == sectionid)).FirstOrDefault();

            var getdesk = await _context.DeskCounters.FirstOrDefaultAsync(c => c.QMSUserId == userId);
            if (getdesk == null)
            {
                //return home
                return RedirectToAction(nameof(Index));
            }

            var phonenumber = "";
            var calltime = "";
            var waitingtime = "";
            var servingid = "";
            var myJson = new JObject();
            //var myJson8 = new JObject();

            if (!string.IsNullOrEmpty(jsonData.TicketReports))
            {
                var myJson1 = JObject.Parse(jsonData.TicketReports);
                foreach (var jname in myJson1)
                {
                    myJson.Add(jname.Key, jname.Value);
                    if (jname.Key == currentmember)
                    {
                        phonenumber = jname.Value.Value<dynamic>("PhoneNumber");
                        calltime = jname.Value.Value<dynamic>("CallTime");
                        waitingtime = jname.Value.Value<dynamic>("WaitingTime");
                        servingid = jname.Value.Value<dynamic>("Serviceid");
                        break;
                    }
                }
            }
            var getemail = await _context.QMSUsers.FirstOrDefaultAsync(c => c.Id == userId);
            //close the next member
            var jsonSample = JObject.Parse(_context.Tickets.Where(x => x.Created == today).ToListAsync().Result.First().TicketReports);
            jsonSample[currentmember]["LeftTime"] = currenttime;
            jsonSample[currentmember]["Status"] = "1";


            var membercalltime = Convert.ToDateTime(calltime);
            var membercurrenttime = Convert.ToDateTime(currenttime);
            var getdifference = (membercurrenttime - membercalltime);

            jsonSample[currentmember]["ServedTime"] = getdifference;
            jsonSample[currentmember]["NowServing"] = "0";
            jsonSample[currentmember]["Closed"] = "1";
            jsonSample[currentmember]["TurnOut"] = "1";
            jsonSample[currentmember]["ServedBy"] = getemail.FullName;

            var assijgn = await _context.Tickets.FindAsync(currentday.Id);

            assijgn.TicketReports = jsonSample.ToString();

            _context.Update(assijgn);
            await _context.SaveChangesAsync();
            //updateuser

            
            var user = await _context.QMSUsers.FindAsync(userId);
            user.Created = today;
            user.NowServing = null;
            user.NowServingSecret = null;
            //user.RatingToday = nxtticket;
            _context.Update(user);
            await _context.SaveChangesAsync();


            //update screen
            var getscreeen = await _context.Screens.FirstOrDefaultAsync(c => c.DeskCounterId == getdesk.Id);
            var updatethescreen = await _context.Screens.FindAsync(getscreeen.Id);
            updatethescreen.TicketNumber = "None";
            updatethescreen.Status = "Free";
            _context.Update(updatethescreen);
            await _context.SaveChangesAsync();


            //update report

            var getmin = waitingtime;
            string hour = getmin.Substring(0, 2);
            string min = getmin.Substring(3, 5);
            string truemin = min.Substring(0, 2);

            int hr = Int32.Parse(hour);
            int minutes = Int32.Parse(truemin);
            int totalwaiting = 0;

            if (hr > 0)
            {
                int totalmin = (hr * 60);
                totalwaiting = (totalmin + minutes);
            }
            else
            {
                totalwaiting = minutes;
            }


            var getminser = getdifference.ToString();
            string hourser = getminser.Substring(0, 2);
            string minser = getminser.Substring(3, 5);
            string trueminser = minser.Substring(0, 2);

            int hrser = Int32.Parse(hourser);
            int minutesser = Int32.Parse(trueminser);
            int totalser = 0;

            if (hrser > 0)
            {
                int totalminser = (hrser * 60);
                totalser = (totalminser + minutesser);
            }
            else
            {
                totalser = minutesser;
            }
            //update report
            var getrep = await _context.Reports.FirstOrDefaultAsync(c => c.Created == today);
            getrep.TotalServed = (getrep.TotalServed + 1);
            getrep.TotalWaitingTime = (getrep.TotalWaitingTime + totalwaiting);
            getrep.TotalServingTime = (getrep.TotalServingTime + totalser);
            _context.Reports.Update(getrep);
            await _context.SaveChangesAsync();


            //update section
            var getsecc = await _context.SectionReports.FirstOrDefaultAsync(c => (c.SectionId == sectionid && c.Created == today));
            getsecc.TotalServed = (getsecc.TotalServed + 1);
            getsecc.TotalWaitingTime = (getsecc.TotalWaitingTime + totalwaiting);
            getsecc.TotalServingTime = (getsecc.TotalServingTime + totalser);
            _context.SectionReports.Update(getsecc);
            await _context.SaveChangesAsync();


            //update service
            var getser = await _context.ServicesReports.FirstOrDefaultAsync(c => (c.ServiceId == servingid && c.Created == today));
            getser.TotalServed = (getser.TotalServed + 1);
            getser.TotalWaitingTime = (getser.TotalWaitingTime + totalwaiting);
            getser.TotalServingTime = (getser.TotalServingTime + totalser);
            _context.ServicesReports.Update(getser);
            await _context.SaveChangesAsync();

            //update qmsuserservice
            var getqmsu = await _context.QMSUserService.FirstOrDefaultAsync(c => (c.ServiceId == servingid && c.QMSUserId == userId));
            var uptqms = await _context.QMSUserService.FindAsync(getqmsu.Id);
            uptqms.ServedAllTime = (uptqms.ServedAllTime + 1);
            _context.QMSUserService.Update(uptqms);
            await _context.SaveChangesAsync();

            //update user
            var upddateuser = await _context.QMSUsers.FindAsync(userId);
            upddateuser.ServedAlltime = (upddateuser.ServedAlltime + 1);
            upddateuser.ServedToday = (upddateuser.ServedToday + 1);
            upddateuser.RatingAllTime = (upddateuser.RatingAllTime + 5);
            upddateuser.RatingToday = (upddateuser.RatingToday + 5);
            _context.QMSUsers.Update(upddateuser);
            await _context.SaveChangesAsync();

            //update ticket
            var updatetick = await _context.Tickets.FirstOrDefaultAsync(c => (c.SectionId == sectionid && c.Created == today));
            var upttic = await _context.Tickets.FindAsync(updatetick.Id);
            upttic.ServedMembers = (upttic.ServedMembers + 1);
            _context.Tickets.Update(upttic);
            await _context.SaveChangesAsync();


            await _signalrHub.Clients.All.SendAsync("TotalMembers");
            await _signalrHub.Clients.All.SendAsync("LoadTickets");
            await _signalrHub.Clients.All.SendAsync("LoadScreen");
            await _signalrHub.Clients.All.SendAsync("LoadScreenLower");


            //send sms to ask him in
            var smsJsonPost = new JObject
                                            {
                                                { "api_key", "5fae8de671e67" },
                                                { "username", "maskani" },
                                                { "sender_id", "22136" },
                                                { "message", "Thankyou for visiting Nyati Sacco, for self service *346#. You were served by "+getemail.FullName.ToUpper()+", rate the service on https://nyatiqms.com/ expires in 10 minutes" },
                                                { "phone", phonenumber}
                                            };

            var smsData = smsJsonPost;
            var smsEndPointUrl = "http://bulksms.mobitechtechnologies.com/api/sendsms";
            var smsResponse = SendPostRequest(smsData, smsEndPointUrl);



            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Getwaitingmembers()
        {
            var today = DateTime.Now.ToString("dd/MM/yyyy");
            int count = 0;
            var res = _context.Tickets.Where(c => (c.Created == today)).ToList();
            foreach(var item in res)
            {
                count += item.WaitingMembers;
            }
            return Ok(count);
        }
        [HttpGet]
        public IActionResult GetChatCount()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            int countunread = 0;
            var getallstaff = _context.QMSUsers.Where(c => c.Id == userId).ToList();
            foreach(var item in getallstaff)
            {
                if(item.Notifications != null)
                {
                    var userCurrentt = _context.QMSUsers.FirstOrDefault(c => c.Id == item.Id);
                    var check = userCurrentt.Notifications;
                    var myJson = new JObject();
                    if (check != null)
                    {
                        ViewBag.notifications = JObject.Parse(userCurrentt.Notifications);
                        var myJson1 = JObject.Parse(userCurrentt.Notifications);
                        foreach (var jname in myJson1)
                        {
                            myJson.Add(jname.Key, jname.Value);
                            if (jname.Value.Value<string>("Read") == "0")
                            {
                                countunread++;
                            }
                        }
                    }
                }
            }
            

            return Ok(countunread);
        }
        [HttpGet]
        public IActionResult Getservedmembers()
        {
            var today = DateTime.Now.ToString("dd/MM/yyyy");
            var res = _context.TodaysTickets.Where(c => (c.Served == true)).Count();
            
            return Ok(res);
        }
        [HttpGet]
        public IActionResult Getbeingservedmembers()
        {
            var today = DateTime.Now.ToString("dd/MM/yyyy");
            var res = _context.TodaysTickets.Where(c => (c.Serving == true)).Count();

            return Ok(res);
        }
        [HttpGet]
        public IActionResult Getstaffservingmembers()
        {
            var today = DateTime.Now.ToString("dd/MM/yyyy");
            var res = _context.TodaysTickets.Where(c => (c.Serving == true)).Count();

            return Ok(res);
        }
        [HttpGet]
        public IActionResult GetTickets()
        {
            var today = DateTime.Now.ToString("dd/MM/yyyy");

            var res = _context.TodaysTickets
                .Where(c => (c.Created == today && c.Served == false && c.Serving == false))
                .Include(t => t.Section).Include(t => t.Service)
            .Take(5).ToList().OrderByDescending(c => c.OrderNumber);
            return Ok(res);
        }
        [HttpGet]
        public async Task<IActionResult> QueueMembers(string id)
        {
            var today = DateTime.Now.ToString("dd/MM/yyyy");

            var checktype = await _context.Tickets.Where(c => c.Created == today).FirstOrDefaultAsync();
            if(checktype != null)
            {
                var sec = await _context.Sections.FirstOrDefaultAsync(c => c.Id == checktype.SectionId);

                ViewBag.secname = sec.Name;
                var jsonData = _context.Tickets.Where(c => c.Created == today).FirstOrDefault();

                var myJson = new JObject();
                //var myJson8 = new JObject();

                if (!string.IsNullOrEmpty(jsonData.TicketReports))
                {
                    var myJson1 = JObject.Parse(jsonData.TicketReports);
                    foreach (var jname in myJson1)
                    {
                        myJson.Add(jname.Key, jname.Value);
                    }
                }
                ViewBag.Members = myJson;

            }
            return View();
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
        public static async Task<string> SendPostRequest1(JObject data1, string url)
        {

            string myJson = data1.ToString();
            string smsPostResponse1 = "";
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(
                    url,
                     new StringContent(myJson, Encoding.UTF8, "application/json"));
                smsPostResponse1 = response.StatusCode.ToString();
            }

            return smsPostResponse1;
        }

    }
}
