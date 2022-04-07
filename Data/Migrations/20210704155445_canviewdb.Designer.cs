﻿// <auto-generated />
using System;
using HubnyxQMS.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HubnyxQMS.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20210704155445_canviewdb")]
    partial class canviewdb
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.6")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("HubnyxQMS.Models.QMSRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("QMSUserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.HasIndex("QMSUserId");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("HubnyxQMS.Models.QMSUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("AverageServingTime")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("CanViewDashboard")
                        .HasColumnType("bit");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Created")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NowServing")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NowServingSecret")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("ProfilePicture")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RatingAllTime")
                        .HasColumnType("int");

                    b.Property<int>("RatingToday")
                        .HasColumnType("int");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ServedAlltime")
                        .HasColumnType("int");

                    b.Property<int>("ServedToday")
                        .HasColumnType("int");

                    b.Property<bool>("ServingMember")
                        .HasColumnType("bit");

                    b.Property<double>("SuccessRateAlltime")
                        .HasColumnType("float");

                    b.Property<double>("SuccessRateToday")
                        .HasColumnType("float");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("HubnyxQMS.Models.QMSUserService", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("QMSUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ServiceId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("QMSUserId");

                    b.HasIndex("ServiceId");

                    b.ToTable("QMSUserService");
                });

            modelBuilder.Entity("HubnyxQMS.Models.Section", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Sections");
                });

            modelBuilder.Entity("HubnyxQMS.Models.Service", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SectionId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("SectionId");

                    b.ToTable("Services");
                });

            modelBuilder.Entity("HubnyxQMS.Models.Ticket", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AverageServingTime")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AverageWaitingTime")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Closed")
                        .HasColumnType("bit");

                    b.Property<string>("Created")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MembersBeingServed")
                        .HasColumnType("int");

                    b.Property<int>("NoTurnOutMembers")
                        .HasColumnType("int");

                    b.Property<int>("Ratings")
                        .HasColumnType("int");

                    b.Property<string>("RegisteredDevice")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SectionId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("ServedMembers")
                        .HasColumnType("int");

                    b.Property<string>("TicketReports")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TotalEscalated")
                        .HasColumnType("int");

                    b.Property<int>("TotalMembers")
                        .HasColumnType("int");

                    b.Property<int>("WaitingMembers")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SectionId");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("HubnyxQMS.Models.TodaysTicket", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CheckInTime")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Created")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OrderNumber")
                        .HasColumnType("int");

                    b.Property<string>("SectionId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("Served")
                        .HasColumnType("bit");

                    b.Property<string>("ServiceId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("Serving")
                        .HasColumnType("bit");

                    b.Property<string>("TicketNumber")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("SectionId");

                    b.HasIndex("ServiceId");

                    b.ToTable("TodaysTickets");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("HubnyxQMS.Models.QMSRole", b =>
                {
                    b.HasOne("HubnyxQMS.Models.QMSUser", null)
                        .WithMany("QMSRoles")
                        .HasForeignKey("QMSUserId");
                });

            modelBuilder.Entity("HubnyxQMS.Models.QMSUserService", b =>
                {
                    b.HasOne("HubnyxQMS.Models.QMSUser", "QMSUser")
                        .WithMany()
                        .HasForeignKey("QMSUserId");

                    b.HasOne("HubnyxQMS.Models.Service", "Service")
                        .WithMany()
                        .HasForeignKey("ServiceId");

                    b.Navigation("QMSUser");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("HubnyxQMS.Models.Service", b =>
                {
                    b.HasOne("HubnyxQMS.Models.Section", "Section")
                        .WithMany()
                        .HasForeignKey("SectionId");

                    b.Navigation("Section");
                });

            modelBuilder.Entity("HubnyxQMS.Models.Ticket", b =>
                {
                    b.HasOne("HubnyxQMS.Models.Section", "Section")
                        .WithMany()
                        .HasForeignKey("SectionId");

                    b.Navigation("Section");
                });

            modelBuilder.Entity("HubnyxQMS.Models.TodaysTicket", b =>
                {
                    b.HasOne("HubnyxQMS.Models.Section", "Section")
                        .WithMany()
                        .HasForeignKey("SectionId");

                    b.HasOne("HubnyxQMS.Models.Service", "Service")
                        .WithMany()
                        .HasForeignKey("ServiceId");

                    b.Navigation("Section");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("HubnyxQMS.Models.QMSRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("HubnyxQMS.Models.QMSUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("HubnyxQMS.Models.QMSUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("HubnyxQMS.Models.QMSRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HubnyxQMS.Models.QMSUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("HubnyxQMS.Models.QMSUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("HubnyxQMS.Models.QMSUser", b =>
                {
                    b.Navigation("QMSRoles");
                });
#pragma warning restore 612, 618
        }
    }
}
