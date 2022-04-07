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
//using DinkToPdf;
using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace HubnyxQMS.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IConverter _converter;
        private readonly UserManager<QMSUser> _userManager;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(ApplicationDbContext context, ILogger<ReportsController> logger, IConverter converter, UserManager<QMSUser> userManager)
        {
            _context = context;
            _converter = converter;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CreatePDF(string id, string buildingSlug, string year, string month, string buildingRef, string roomnumber)
        {

            year = "2020";
            month = "jan";
            roomnumber = "xxxx";
            var logo = Request.Scheme + "://" + Request.Host + "/7.jpg";
            var managementlogo = logo;
            
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "QMS Report"
            };

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var getemail = await _context.QMSUsers.FirstOrDefaultAsync(c => c.Id == userId);
            ViewBag.email = getemail.Email;
            ViewBag.date = DateTime.Now.ToString("dd/MMM/yyyy");

            var currentdate = DateTime.Now.ToString("dd/MM/yyyy");

            //get total
            int totalmembers = 0;
            int totalserved = 0;
            int totalaveragewaiting = 0;
            int totalaverageserving = 0;
            int rating = 0;
            int count = 0;
            var getallmembers = await _context.Reports.ToListAsync();
            foreach (var item in getallmembers)
            {
                count++;
                totalmembers += item.TotalMembers;
                totalserved += item.TotalServed;
                totalaveragewaiting += item.TotalWaitingTime;
                totalaverageserving += item.TotalServingTime;
                rating += item.Rating;
            }
            ViewBag.totalmembers = totalmembers;
            ViewBag.totalserved = totalserved;
            ViewBag.average = (totalaveragewaiting / totalserved).ToString();
            ViewBag.averageserving = (totalaverageserving / totalserved).ToString();
            ViewBag.notserved = (totalmembers - totalserved);
            ViewBag.rating = (rating / count).ToString();


            int totalescalated = 0;
            var getallescalated = await _context.Escalates.ToListAsync();
            foreach (var item in getallescalated)
            {
                totalescalated++;
            }
            ViewBag.totalescalated = totalescalated;


            var sectin = await _context.Sections.FirstOrDefaultAsync(c => c.Name == "BOSA");
            ViewBag.bosa = await _context.Services.Where(c => c.SectionId == sectin.Id).ToListAsync();
            ViewBag.bosaserv = await _context.ServicesReports.ToListAsync();

            var sectinf = await _context.Sections.FirstOrDefaultAsync(c => c.Name == "FOSA");
            ViewBag.fosa = await _context.Services.Where(c => c.SectionId == sectinf.Id).ToListAsync();


            //get member with most members
            int max = 0;
            var allusers = await _context.QMSUsers.ToListAsync();
            foreach (var maximum in allusers)
            {

                if (maximum.ServedAlltime > max)
                {
                    max = maximum.ServedAlltime;
                }
            }
            ViewBag.max = max;
            var getstaffwith = await _context.QMSUsers.FirstOrDefaultAsync(c => c.ServedAlltime == max);
            ViewBag.mostserved = getstaffwith.FullName;



            //count day with most members
            int mostdays = 0;
            int averagevisits = 0;

            var alldays = await _context.Reports.ToListAsync();
            foreach (var maximum in alldays)
            {
                averagevisits += maximum.TotalMembers;
                if (maximum.TotalMembers > mostdays)
                {
                    mostdays = maximum.TotalMembers;
                }
            }
            ViewBag.most = mostdays;
            var getmostdayswith = await _context.Reports.FirstOrDefaultAsync(c => c.TotalMembers == mostdays);
            ViewBag.mostdays = getmostdayswith.Created;

            //calculate average visits
            var getalldays = await _context.Reports.CountAsync();
            ViewBag.averagevisits = (averagevisits / getalldays);



            //bosa report
            var getbosaname = await _context.Sections.FirstOrDefaultAsync(c => c.Name == "BOSA");
            int bosatotalmembers = 0;
            int bosatotalserved = 0;
            int bosatotalaveragewaiting = 0;
            int bosatotalaverageserving = 0;
            int bosarating = 0;
            int bosacount = 0;
            int bosaescalated = 0;
            var bosagetallmembers = await _context.SectionReports.Where(c => c.SectionId == getbosaname.Id).ToListAsync();
            foreach (var item in bosagetallmembers)
            {
                bosaescalated += item.EscalatedCases;
                bosacount++;
                bosatotalmembers += item.TotalMembers;
                bosatotalserved += item.TotalServed;
                bosatotalaveragewaiting += item.TotalWaitingTime;
                bosatotalaverageserving += item.TotalServingTime;
                bosarating += item.Rating;
            }
            ViewBag.bosatotalmembers = bosatotalmembers;
            ViewBag.bosaescalated = bosaescalated;
            ViewBag.bosatotalserved = bosatotalserved;
            ViewBag.bosanotserved = (bosatotalmembers - bosatotalserved);


            try
            {
                ViewBag.bosaaverage = (bosatotalaveragewaiting / bosatotalserved).ToString();
                ViewBag.bosaaverageserving = (bosatotalaverageserving / bosatotalserved).ToString();
                ViewBag.bosarating = (bosarating / bosacount).ToString();
            }
            catch
            {
                ViewBag.bosaaverage = "0";
                ViewBag.bosarating = "0";
                ViewBag.bosaaverageserving = "0";
            }



            int bosadays = 0;
            int bosaaveragevisits = 0;

            var allbosa = await _context.SectionReports.Where(c => c.SectionId == getbosaname.Id).ToListAsync();
            foreach (var maximum in allbosa)
            {
                bosaaveragevisits += maximum.TotalMembers;
                if (maximum.TotalMembers > bosadays)
                {
                    bosadays = maximum.TotalMembers;
                }
            }
            ViewBag.bosamost = bosadays;
            var bosamostdayswith = await _context.SectionReports.FirstOrDefaultAsync(c => c.TotalMembers == mostdays);
            ViewBag.bosamostdays = getmostdayswith.Created;

            var bosagetalldays = await _context.SectionReports.Where(c => c.SectionId == getbosaname.Id).CountAsync();
            ViewBag.bosaaveragevisits = (bosaaveragevisits / bosagetalldays);

            //bosa most served
            int maxbosa = 0;
            var allusersbosa = await _context.QMSUsers.Where(c => c.NowServingSectionId == getbosaname.Id).ToListAsync();
            foreach (var maximum in allusersbosa)
            {

                if (maximum.ServedAlltime > maxbosa)
                {
                    maxbosa = maximum.ServedAlltime;
                }
            }
            ViewBag.mabosax = maxbosa;
            var getstaffwithbosa = await _context.QMSUsers.FirstOrDefaultAsync(c => c.ServedAlltime == maxbosa);
            ViewBag.mostservedbosa = getstaffwithbosa.FullName;


            //fosa report
            var getfosaname = await _context.Sections.FirstOrDefaultAsync(c => c.Name == "FOSA");
            int fosatotalmembers = 0;
            int fosatotalserved = 0;
            int fosatotalaveragewaiting = 0;
            int fosatotalaverageserving = 0;
            int fosarating = 0;
            int fosacount = 0;
            int fosaescalated = 0;
            var fosagetallmembers = await _context.SectionReports.Where(c => c.SectionId == getfosaname.Id).ToListAsync();
            foreach (var item in fosagetallmembers)
            {
                fosacount++;
                fosaescalated += item.EscalatedCases;
                fosatotalmembers += item.TotalMembers;
                fosatotalserved += item.TotalServed;
                fosatotalaveragewaiting += item.TotalWaitingTime;
                fosatotalaverageserving += item.TotalServingTime;
                fosarating += item.Rating;
            }
            ViewBag.fosatotalmembers = fosatotalmembers;
            ViewBag.fosatotalserved = fosatotalserved;
            ViewBag.fosanotserved = (fosatotalmembers - fosatotalserved);
            ViewBag.fosaescalated = fosaescalated;
            try
            {
                ViewBag.fosarating = (fosarating / fosacount).ToString();
                ViewBag.fosaaverage = (fosatotalaveragewaiting / fosatotalserved).ToString();
                ViewBag.fosaaverageserving = (fosatotalaverageserving / fosatotalserved).ToString();
            }
            catch
            {
                ViewBag.fosarating = "0";
                ViewBag.fosaaverage = "0";
                ViewBag.fosaaverageserving = "0";
            }

            int fosadays = 0;
            int fosaaveragevisits = 0;

            var allfosa = await _context.SectionReports.Where(c => c.SectionId == getfosaname.Id).ToListAsync();
            foreach (var maximum in allfosa)
            {
                fosaaveragevisits += maximum.TotalMembers;
                if (maximum.TotalMembers > fosadays)
                {
                    fosadays = maximum.TotalMembers;
                }
            }
            ViewBag.fosamost = fosadays;
            var fosamostdayswith = await _context.SectionReports.FirstOrDefaultAsync(c => c.TotalMembers == mostdays);
            ViewBag.fosamostdays = getmostdayswith.Created;

            var fosagetalldays = await _context.SectionReports.Where(c => c.SectionId == getfosaname.Id).CountAsync();
            ViewBag.fosaaveragevisits = (fosaaveragevisits / fosagetalldays);

            //fosa most served
            int maxfosa = 0;
            var allusersfosa = await _context.QMSUsers.Where(c => c.NowServingSectionId == getfosaname.Id).ToListAsync();
            foreach (var maximum in allusersfosa)
            {

                if (maximum.ServedAlltime > maxfosa)
                {
                    maxfosa = maximum.ServedAlltime;
                }
            }
            ViewBag.mafosax = maxfosa;
            var getstaffwithfosa = await _context.QMSUsers.FirstOrDefaultAsync(c => c.ServedAlltime == maxfosa);
            ViewBag.mostservedfosa = getstaffwithfosa.FullName;

            var zero = "0";
            try
            {
                var sb = new StringBuilder();
                sb.Append(@"<!doctype html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Receipt</title>
    
</head>

<body>
                          <div class='invoice-box'>
                                <table cellpadding='0' cellspacing='0'>
                                    <tr class='top'>
                                        <td width='600'>
                                            <table style='text-align: right;'>
                                                <tr>
                                                    <td class='title'>" +
                                                        "<img src='" + managementlogo + "' style='width:150px; max-width:150px;'>" +
                                                    "</td>" +
                                                "</tr>" +
                                            "</table>" +
                                        "</td>" +
                                    "</tr>" +
                                    "<h4>Full Report</h4>" +
                                    "<tr class='information'>" +
                                        "<td>" +
                                            "<table>" +
                                                "<tr>" +
                                                    "<td> <h4>General Report</h4>" +
                                                        "Total Members:" +
                                                        totalmembers +
                                                        "<br>" +
                                                        "Total Served:" +
                                                        totalserved +
                                                        "<br>" +
                                                        "AVG Serv Time:  " +
                                                        totalaveragewaiting +
                                                        "<br>" +
                                                        "AVG Wait Time:  " +
                                                        totalaverageserving +
                                                    " Min</td>" +

                                                "</tr>" +
                                            "</table>" +
                                        "</td>" +
                                        "<td></td>" +
                                        "<td>" +
                                            "<table>" +
                                                "<tr>" +
                                                    "<td><h4>BOSA Report</h4>" +
                                                        "Total Members:" +
                                                        bosatotalmembers +
                                                        "<br>" +
                                                        "Total Served:" +
                                                        bosatotalserved +
                                                        "<br>" +
                                                        "AVG Serv Time:" +
                                                        bosatotalaverageserving +
                                                        "<br>" +
                                                        "AVG Wait Time:" +
                                                        bosatotalaveragewaiting +
                                                    "</td>" +
                                                "</tr>" +
                                            "</table>" +
                                        "</td>" +
                                        "<td></td>" +
                                         "<td>" +
                                            "<table>" +
                                                "<tr>" +
                                                    "<td><h4>BOSA Report</h4>" +
                                                        "Total Members:" +
                                                        fosatotalmembers +
                                                        "<br>" +
                                                        "Total Served:" +
                                                        fosatotalserved +
                                                        "<br>" +
                                                        "AVG Serv Time:" +
                                                        fosatotalaverageserving +
                                                        "<br>" +
                                                        "AVG Wait Time:" +
                                                        fosatotalaveragewaiting +
                                                    "</td>" +
                                                "</tr>" +
                                            "</table>" +

                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='heading'>" +
                                        "<td width='300' align='left'>" +
                                            "General Report" +
                                        "</td>" +
                                         "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td width='300' align='right'>" +
                                            "Status" +
                                        "</td>" +
                                    "</tr>" +






                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Total Members" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'>" +
                                            totalmembers +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Total Served" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> " +
                                        totalserved +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Total Not Served" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> " +
                                        (totalmembers - totalserved) +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Average Rating" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> " +
                                        zero +
                                        "/5</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Average Waiting time" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> " +
                                        totalaveragewaiting +
                                        " Min</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Average Serving time" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> " +
                                        totalaverageserving +
                                        " Min</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Total Escalated Cases" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> " +
                                        totalescalated +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Date with most members" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> " +
                                         max+" - "+getstaffwith.FullName+""+
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Average Visits per day" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> " +
                                         averagevisits+
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Staff Who Served Most Members" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> " +
                                         max + " - " + getstaffwith.FullName + "" +
                                        "</td>" +
                                    "</tr>" +






                                    "<tr class='heading'>" +
                                        "<td width='300' align='left'>" +
                                            "BOSA Report" +
                                        "</td>" +
                                         "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td width='300' align='right'>" +
                                            "Status" +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Total Members" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'>" +
                                            totalmembers +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Total Served" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> " +
                                        totalmembers +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Total Not Served" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> " +
                                        totalmembers +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Average Rating" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> " +
                                        totalmembers +
                                        "/5</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Average Waiting time" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> " +
                                        totalmembers +
                                        " Min</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Average Serving time" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> " +
                                        totalmembers +
                                        " Min</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Total Escalated Cases" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> " +
                                        totalmembers +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Date with most members" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> 0" +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Date with least members" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> 0" +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Average Visits per day" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> 0" +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Average Staff Success rate" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> 0" +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Staff Who Served Most Members" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> 0" +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Staff Who Served Least Members" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> 0" +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Staff Who Served with shortest time" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> 0" +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Staff Who Served with most time" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> 0" +
                                        "</td>" +
                                    "</tr>" +


                                    "<tr class='heading'>" +
                                        "<td width='300' align='left'>" +
                                            "FOSA Report" +
                                        "</td>" +
                                         "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td width='300' align='right'>" +
                                            "Status" +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Total Members" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'>" +
                                            totalmembers +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Total Served" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> " +
                                        totalmembers +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Total Not Served" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> " +
                                        totalmembers +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Average Rating" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> " +
                                        totalmembers +
                                        "/5</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Average Waiting time" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> " +
                                        totalmembers +
                                        " Min</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Average Serving time" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> " +
                                        totalmembers +
                                        " Min</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Total Escalated Cases" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> " +
                                        totalmembers +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Date with most members" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> 0" +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Date with least members" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> 0" +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Average Visits per day" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> 0" +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Average Staff Success rate" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> 0" +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Staff Who Served Most Members" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> 0" +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Staff Who Served Least Members" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> 0" +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Staff Who Served with shortest time" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> 0" +
                                        "</td>" +
                                    "</tr>" +
                                    "<tr class='details'>" +
                                        "<td width='300' align='left'>" +
                                            "Staff Who Served with most time" +
                                        "</td>" +

                                           "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +
                                        "<td></td>" +

                                        "<td width='300' align='right'> 0" +
                                        "</td>" +
                                    "</tr>" +


                                    "<tr class='heading'>" +
                                        "<td align='left'>" +
                                            "BOSA Services" +
                                        "</td>" +

                                         "<td width='250' align='right'>" +
                                            "Total" +
                                        "</td>" +

                                         "<td width='250' align='right'>" +
                                            "Served" +
                                        "</td>" +

                                        "<td width='250' align='right'>" +
                                            "AvgWaitTm" +
                                        "</td>" +

                                         "<td width='250' align='right'>" +
                                            "AvgServeTm" +
                                        "</td>" +

                                         "<td width='250' align='right'>" +
                                            "Escalated" +
                                        "</td>" +
                                    "</tr>");
                var getsecc = await _context.Sections.FirstOrDefaultAsync(c => c.Name == "BOSA");
                var getBosaservices = await _context.Services.Where(c => c.SectionId == getsecc.Id).ToListAsync();
                                    foreach (var item in getBosaservices)
                                    {
                    
                    sb.Append(@"<tr class='item'>" +
                                        "<td align='left'>" +
                                            item.Name +
                                        "</td>" +

                                        "<td width='250' align='right'>" +
                                        totalmembers +
                                        "</td>" +

                                          "<td width='250' align='right'>" +
                                          totalmembers +
                                        "</td>" +
                                        "<td width='250' align='right'>" +
                                        totalmembers +
                                        "</td>" +
                                        "<td width='250' align='right'>" +
                                        totalmembers +
                                        "</td>" +

                                        "<td width='250' align='right'>" +
                                        totalmembers +
                                        "</td>" +
                                    "</tr>");
                                    }




                sb.Append(@"<tr class='heading'>" +
                                        "<td align='left'>" +
                                            "FOSA Services" +
                                        "</td>" +

                                         "<td width='250' align='right'>" +
                                            "Total" +
                                        "</td>" +

                                         "<td width='250' align='right'>" +
                                            "Served" +
                                        "</td>" +

                                        "<td width='250' align='right'>" +
                                            "AvgWaitTm" +
                                        "</td>" +

                                         "<td width='250' align='right'>" +
                                            "AvgServeTm" +
                                        "</td>" +

                                         "<td width='250' align='right'>" +
                                            "Escalated" +
                                        "</td>" +
                                    "</tr>");
                var getsec = await _context.Sections.FirstOrDefaultAsync(c => c.Name == "FOSA");
                var getFosaservices = await _context.Services.Where(c => c.SectionId == getsec.Id).ToListAsync();
                foreach (var item in getFosaservices)
                                    {
                    
                    sb.Append(@"<tr class='item'>" +
                                        "<td align='left'>" +
                                            item.Name +
                                        "</td>" +

                                        "<td width='250' align='right'>" +
                                        totalmembers +
                                        "</td>" +

                                          "<td width='250' align='right'>" +
                                          totalmembers +
                                        "</td>" +
                                        "<td width='250' align='right'>" +
                                        totalmembers +
                                        "</td>" +
                                        "<td width='250' align='right'>" +
                                        totalmembers +
                                        "</td>" +

                                        "<td width='250' align='right'>" +
                                        totalmembers +
                                        "</td>" +
                                    "</tr>");
                }
                    
                    sb.Append(@"<tr class='item'>" +
                    "<td align='left' valign='centre'>" +
                    "<strong>Nyati Sacco QMS</strong><i></i>" +
                    "</td>" +
                     "<td width='250' align='right'></td>" +
                                           "<td width='250' align='right'>" +
                                        "<td></td>" +
                    "<td align='right'><img src = '" + logo + "' style = 'width:60px; max-width:60px;float:right;'></td>" +
                    "</tr>" +
                "</table>" +
            "</div></body>" +
    "</html>");
                    var objectSettings = new ObjectSettings
                    {
                        PagesCount = true,
                        HtmlContent = sb.ToString(),
                        WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "receipt.css") },
                        HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "", Line = false },
                        FooterSettings = { FontName = "Arial", FontSize = 9, Line = false, Center = "Nyati Sacco QMS - ReportGenerated on " + DateTime.UtcNow.AddHours(3) }

                        //PagesCount = false,
                        //HtmlContent = sb.ToString(),
                        //WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "receipt.css") },
                        //HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Receipt", Line = false },
                        //FooterSettings = { FontName = "Arial", FontSize = 9, Line = false, Center = "eMaskani Systems Ltd - Report Generated on " + DateTime.Now }
                    };
                    var pdf = new HtmlToPdfDocument()
                    {
                        GlobalSettings = globalSettings,
                        Objects = { objectSettings }
                    };
                    var thename = "NyatiQmsReport";
                    var file = _converter.Convert(pdf);
                    return File(file, "application/pdf", thename + ".pdf");
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex.Message);
                    //return View();
                    return File("", "application/pdf", "error.pdf");
                }
            
            

        }

        // GET: Reports
        public async Task<IActionResult> Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var getemail = await _context.QMSUsers.FirstOrDefaultAsync(c => c.Id == userId);
            ViewBag.email = getemail.Email;
            ViewBag.date = DateTime.Now.ToString("dd/MMM/yyyy");

            var currentdate = DateTime.Now.ToString("dd/MM/yyyy");

            //get total
            int totalmembers = 0;
            int totalserved = 0;
            int totalaveragewaiting = 0;
            int totalaverageserving = 0;
            int rating = 0;
            int count = 0;
            var getallmembers = await _context.Reports.ToListAsync();
            foreach(var item in getallmembers)
            {
                count++;
                totalmembers += item.TotalMembers;
                totalserved += item.TotalServed;
                totalaveragewaiting += item.TotalWaitingTime;
                totalaverageserving += item.TotalServingTime;
                rating += item.Rating;
            }
            ViewBag.totalmembers = totalmembers;
            ViewBag.totalserved = totalserved;
            ViewBag.average = (totalaveragewaiting / totalserved).ToString();
            ViewBag.averageserving = (totalaverageserving / totalserved).ToString();
            ViewBag.notserved = (totalmembers - totalserved);
            ViewBag.rating = (rating / count).ToString();


            int totalescalated = 0;
            var getallescalated = await _context.Escalates.ToListAsync();
            foreach(var item in getallescalated)
            {
                totalescalated++;
            }
            ViewBag.totalescalated = totalescalated;


            var sectin = await _context.Sections.FirstOrDefaultAsync(c => c.Name == "BOSA");
            ViewBag.bosa = await _context.Services.Where(c => c.SectionId == sectin.Id).ToListAsync();
            ViewBag.bosaserv = await _context.ServicesReports.ToListAsync();

            var sectinf = await _context.Sections.FirstOrDefaultAsync(c => c.Name == "FOSA");
            ViewBag.fosa = await _context.Services.Where(c => c.SectionId == sectinf.Id).ToListAsync();


            //get member with most members
            int max = 0;
            var allusers = await _context.QMSUsers.ToListAsync();
            foreach(var maximum in allusers)
            {

                if(maximum.ServedAlltime > max)
                {
                    max = maximum.ServedAlltime;
                }
            }
            ViewBag.max = max;
            var getstaffwith = await _context.QMSUsers.FirstOrDefaultAsync(c => c.ServedAlltime == max);
            ViewBag.mostserved = getstaffwith.FullName;

            

            //count day with most members
            int mostdays = 0;
            int averagevisits = 0;
            
            var alldays = await _context.Reports.ToListAsync();
            foreach (var maximum in alldays)
            {
                averagevisits += maximum.TotalMembers;
                if (maximum.TotalMembers > mostdays)
                {
                    mostdays = maximum.TotalMembers;
                }
            }
            ViewBag.most = mostdays;
            var getmostdayswith = await _context.Reports.FirstOrDefaultAsync(c => c.TotalMembers == mostdays);
            ViewBag.mostdays = getmostdayswith.Created;

            //calculate average visits
            var getalldays = await _context.Reports.CountAsync();
            ViewBag.averagevisits = (averagevisits / getalldays);



            //bosa report
            var getbosaname = await _context.Sections.FirstOrDefaultAsync(c => c.Name == "BOSA");
            int bosatotalmembers = 0;
            int bosatotalserved = 0;
            int bosatotalaveragewaiting = 0;
            int bosatotalaverageserving = 0;
            int bosarating = 0;
            int bosacount = 0;
            int bosaescalated = 0;
            var bosagetallmembers = await _context.SectionReports.Where(c => c.SectionId == getbosaname.Id).ToListAsync();
            foreach (var item in bosagetallmembers)
            {
                bosaescalated += item.EscalatedCases;
                bosacount++;
                bosatotalmembers += item.TotalMembers;
                bosatotalserved += item.TotalServed;
                bosatotalaveragewaiting += item.TotalWaitingTime;
                bosatotalaverageserving += item.TotalServingTime;
                bosarating += item.Rating;
            }
            ViewBag.bosatotalmembers = bosatotalmembers;
            ViewBag.bosaescalated = bosaescalated;
            ViewBag.bosatotalserved = bosatotalserved;
            ViewBag.bosanotserved = (bosatotalmembers - bosatotalserved);


            try
            {
                ViewBag.bosaaverage = (bosatotalaveragewaiting / bosatotalserved).ToString();
                ViewBag.bosaaverageserving = (bosatotalaverageserving / bosatotalserved).ToString();
                ViewBag.bosarating = (bosarating / bosacount).ToString();
            }
            catch
            {
                ViewBag.bosaaverage = "0";
                ViewBag.bosarating = "0";
                ViewBag.bosaaverageserving = "0";
            }



            int bosadays = 0;
            int bosaaveragevisits = 0;

            var allbosa = await _context.SectionReports.Where(c => c.SectionId == getbosaname.Id).ToListAsync();
            foreach (var maximum in allbosa)
            {
                bosaaveragevisits += maximum.TotalMembers;
                if (maximum.TotalMembers > bosadays)
                {
                    bosadays = maximum.TotalMembers;
                }
            }
            ViewBag.bosamost = bosadays;
            var bosamostdayswith = await _context.SectionReports.FirstOrDefaultAsync(c => c.TotalMembers == mostdays);
            ViewBag.bosamostdays = getmostdayswith.Created;

            var bosagetalldays = await _context.SectionReports.Where(c => c.SectionId == getbosaname.Id).CountAsync();
            ViewBag.bosaaveragevisits = (bosaaveragevisits / bosagetalldays);

            //bosa most served
            int maxbosa = 0;
            var allusersbosa = await _context.QMSUsers.Where(c => c.NowServingSectionId == getbosaname.Id).ToListAsync();
            foreach (var maximum in allusersbosa)
            {

                if (maximum.ServedAlltime > maxbosa)
                {
                    maxbosa = maximum.ServedAlltime;
                }
            }
            ViewBag.mabosax = maxbosa;
            var getstaffwithbosa = await _context.QMSUsers.FirstOrDefaultAsync(c => c.ServedAlltime == maxbosa);
            ViewBag.mostservedbosa = getstaffwithbosa.FullName;


            //fosa report
            var getfosaname = await _context.Sections.FirstOrDefaultAsync(c => c.Name == "FOSA");
            int fosatotalmembers = 0;
            int fosatotalserved = 0;
            int fosatotalaveragewaiting = 0;
            int fosatotalaverageserving = 0;
            int fosarating = 0;
            int fosacount = 0;
            int fosaescalated = 0;
            var fosagetallmembers = await _context.SectionReports.Where(c => c.SectionId == getfosaname.Id).ToListAsync();
            foreach (var item in fosagetallmembers)
            {
                fosacount++;
                fosaescalated += item.EscalatedCases;
                fosatotalmembers += item.TotalMembers;
                fosatotalserved += item.TotalServed;
                fosatotalaveragewaiting += item.TotalWaitingTime;
                fosatotalaverageserving += item.TotalServingTime;
                fosarating += item.Rating;
            }
            ViewBag.fosatotalmembers = fosatotalmembers;
            ViewBag.fosatotalserved = fosatotalserved;
            ViewBag.fosanotserved = (fosatotalmembers - fosatotalserved);
            ViewBag.fosaescalated = fosaescalated;
            try
            {
                ViewBag.fosarating = (fosarating / fosacount).ToString();
                ViewBag.fosaaverage = (fosatotalaveragewaiting / fosatotalserved).ToString();
                ViewBag.fosaaverageserving = (fosatotalaverageserving / fosatotalserved).ToString();
            }
            catch
            {
                ViewBag.fosarating = "0";
                ViewBag.fosaaverage = "0";
                ViewBag.fosaaverageserving = "0";
            }

            int fosadays = 0;
            int fosaaveragevisits = 0;

            var allfosa = await _context.SectionReports.Where(c => c.SectionId == getfosaname.Id).ToListAsync();
            foreach (var maximum in allfosa)
            {
                fosaaveragevisits += maximum.TotalMembers;
                if (maximum.TotalMembers > fosadays)
                {
                    fosadays = maximum.TotalMembers;
                }
            }
            ViewBag.fosamost = fosadays;
            var fosamostdayswith = await _context.SectionReports.FirstOrDefaultAsync(c => c.TotalMembers == mostdays);
            ViewBag.fosamostdays = getmostdayswith.Created;

            var fosagetalldays = await _context.SectionReports.Where(c => c.SectionId == getfosaname.Id).CountAsync();
            ViewBag.fosaaveragevisits = (fosaaveragevisits / fosagetalldays);

            //fosa most served
            int maxfosa = 0;
            var allusersfosa = await _context.QMSUsers.Where(c => c.NowServingSectionId == getfosaname.Id).ToListAsync();
            foreach (var maximum in allusersfosa)
            {

                if (maximum.ServedAlltime > maxfosa)
                {
                    maxfosa = maximum.ServedAlltime;
                }
            }
            ViewBag.mafosax = maxfosa;
            var getstaffwithfosa = await _context.QMSUsers.FirstOrDefaultAsync(c => c.ServedAlltime == maxfosa);
            ViewBag.mostservedfosa = getstaffwithfosa.FullName;
            return View();
        }

        // GET: Reports/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Reports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }

        // GET: Reports/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Reports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,QMSReport")] Report report)
        {
            if (ModelState.IsValid)
            {
                _context.Add(report);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(report);
        }

        // GET: Reports/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Reports.FindAsync(id);
            if (report == null)
            {
                return NotFound();
            }
            return View(report);
        }

        // POST: Reports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,QMSReport")] Report report)
        {
            if (id != report.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(report);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReportExists(report.Id))
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
            return View(report);
        }

        // GET: Reports/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Reports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }


        //[HttpGet]
        //public async Task<IActionResult> CreatePDF()
        //{
        //    var globalSettings = new GlobalSettings
        //    {
        //        ColorMode = ColorMode.Color,
        //        Orientation = Orientation.Portrait,
        //        PaperSize = PaperKind.A4,
        //        Margins = new MarginSettings { Top = 10 },
        //        DocumentTitle = "PDF Report"
        //    };
        //    var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var getdoneby = await _context.QMSUsers.FirstOrDefaultAsync(c => c.Id == userId);

        //    var reportid = "TheReport";
        //    var jsonDataST = await _context.Reports.Where(c => c.Id == reportid).ToListAsync();

        //    var myJsonst = new JObject();
        //    var totalmembers = "";
        //    var TotalServed = "";
        //    foreach (var memberreport in jsonDataST)
        //    {
        //        if (memberreport.QMSReport != null)
        //        {
        //            var myJson1 = JObject.Parse(memberreport.QMSReport);
        //            foreach (var jname in myJson1)
        //            {
        //                myJsonst.Add(jname.Key, jname.Value);
        //                totalmembers = jname.Value.Value<dynamic>("TotalMembers");
        //                TotalServed = jname.Value.Value<dynamic>("TotalServed");

        //            }
        //        }
        //    }
        //    var today = DateTime.Now.ToString("dd/MM/yyyy");
        //    var sb = new StringBuilder();
        //    sb.Append(@"<html>
        //                    <head>
        //                    </head>
        //                    <body>
        //                        <div class='top-head'>" +
        //                        "<div style='float:right;'><img src='" + Request.Scheme + "://" + Request.Host + "/img/logo.jpg' width='150' height='auto' />" +
        //                        "</div>" +
        //                        "<p>Nyati Sacco</p><p>QMS REPORT.</p>" +
        //                        "</div>" +
        //                        "<div class='header'>" +
        //                            "<h4> PrintedBy: " + getdoneby.Email + "</h4>" +
        //                            "<h4> Date: " + today + "</h4>" +
        //                            "<div class='row'>" +
        //                            "<div class='col-md-4 col-sm-6'>" +
        //                            "<h6>General:</h6>" +
        //                            "<table class='table table-responsive invoice-table invoice-order table-borderless'>" +
        //                                        "<tbody>" +
        //                                            "<tr>" +
        //                                                "<th>Total Members :</th>" +
        //                                                "<td>0</td>" +
        //                                            "</tr>" +
        //                                            "<tr>" +
        //                                                "<th>Total Served :</th>" +
        //                                                "<td>0</td> " +
        //                                            "</tr> " +
        //                                            "<tr> " +
        //                                                "<th>Average waiting time :</th> " +
        //                                                "<td> 0 min</td> " +
        //                                            "</tr> " +
        //                                            "<tr> " +
        //                                               " <th>Average serving time :</th> " +
        //                                                "<td> 0 min</td> " +
        //                                            "</tr> " +
        //                                        "</tbody>" +
        //                                    "</table>" +
        //                            "<div>" +
        //                            "<div class='col-md-4 col-sm-6'>" +
        //                            "<h6>General:</h6>" +
        //                            "<table class='table table-responsive invoice-table invoice-order table-borderless'>" +
        //                                        "<tbody>" +
        //                                            "<tr>" +
        //                                                "<th>Total Members :</th>" +
        //                                                "<td>"+totalmembers+"</td>" +
        //                                            "</tr>" +
        //                                            "<tr>" +
        //                                                "<th>Total Served :</th>" +
        //                                                "<td>"+TotalServed+"</td> " +
        //                                            "</tr> " +
        //                                            "<tr> " +
        //                                                "<th>Average waiting time :</th> " +
        //                                                "<td> 0 min</td> " +
        //                                            "</tr> " +
        //                                            "<tr> " +
        //                                               " <th>Average serving time :</th> " +
        //                                                "<td> 0 min</td> " +
        //                                            "</tr> " +
        //                                        "</tbody>" +
        //                                    "</table>" +
        //                            "<div>" +
        //                            "<div class='col-md-4 col-sm-6'>" +
        //                            "<h6>General:</h6>" +
        //                            "<table class='table table-responsive invoice-table invoice-order table-borderless'>" +
        //                                        "<tbody>" +
        //                                            "<tr>" +
        //                                                "<th>Total Members :</th>" +
        //                                                "<td>0</td>" +
        //                                            "</tr>" +
        //                                            "<tr>" +
        //                                                "<th>Total Served :</th>" +
        //                                                "<td>0</td> " +
        //                                            "</tr> " +
        //                                            "<tr> " +
        //                                                "<th>Average waiting time :</th> " +
        //                                                "<td> 0 min</td> " +
        //                                            "</tr> " +
        //                                            "<tr> " +
        //                                               " <th>Average serving time :</th> " +
        //                                                "<td> 0 min</td> " +
        //                                            "</tr> " +
        //                                        "</tbody>" +
        //                                    "</table>" +
        //                            "<div>" +
        //                            "</div>" +
        //                            "<h4 style='text-align: center;'>Share Capital</h4>" +
        //                        "</div>" +
        //                        "" +
        //                        "" +
        //                        "<table id='customers'><tr><th>Description</th><th></th></tr>" + 
        //                        "<tr> " +
        //                            "<td>Total Members</td>" +
        //                            "<td>" + totalmembers + "</td>" +
        //                        "</tr>" +
        //                        "<tr> " +
        //                            "<td>Total Served Members</td>" +
        //                            "<td>" + TotalServed + "</td>" +
        //                        "</tr>" +
        //                       "</table> " +

        //                    "</body>" +
        //                "</html>");

        //    var todaay = DateTime.Now.AddHours(10).ToString("dddd, dd MMMM yyyy HH:mm:ss");
        //    var objectSettings = new ObjectSettings
        //    {
        //        PagesCount = true,
        //        HtmlContent = sb.ToString(),
        //        WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "style.css") },
        //        HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
        //        FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Nyati Sacco QMS Print: " + todaay + "" }
        //    };
        //    var pdf = new HtmlToPdfDocument()
        //    {
        //        GlobalSettings = globalSettings,
        //        Objects = { objectSettings }
        //    };
        //    var nyatihousing = "Nyati Sacco QMS";
        //    var query = DateTime.Now.AddHours(10).ToString("dd/MM/yyyy hh:mm tt");
        //    var file = _converter.Convert(pdf);
        //    return File(file, "application/pdf", fileDownloadName: nyatihousing.Replace(" ", "_") + "_" + query + ".pdf");
        //}

        // POST: Reports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var report = await _context.Reports.FindAsync(id);
            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReportExists(string id)
        {
            return _context.Reports.Any(e => e.Id == id);
        }
    }
}
