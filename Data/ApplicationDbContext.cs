using HubnyxQMS.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HubnyxQMS.Data
{
    public class ApplicationDbContext : IdentityDbContext<QMSUser, QMSRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<QMSUser> QMSUsers { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<QMSUserService> QMSUserService { get; set; }
        public DbSet<TodaysTicket> TodaysTickets { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<SMSReport> SMSReports { get; set; }
        public DbSet<TabletStatus> TabletStatuses { get; set; }
        public DbSet<Screen> Screens { get; set; }
        public DbSet<InternalComm> InternalComms { get; set; }
        public DbSet<DeskCounter> DeskCounters { get; set; }
        public DbSet<Escalate> Escalates { get; set; }
        public DbSet<ServicesReport> ServicesReports { get; set; }
        public DbSet<SectionReport> SectionReports { get; set; }
        public DbSet<WithDrawal> WithDrawals { get; set; }
        public DbSet<WithDrawalReason> WithDrawalReasons { get; set; }
    }
}
