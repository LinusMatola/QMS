using HubnyxQMS.Data;
using HubnyxQMS.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HubnyxQMS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    //var scope = host.Services.CreateScope();
                    var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<QMSUser>>();
                    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<QMSRole>>();

                    ctx.Database.EnsureCreated();

                    var superAdmin = new QMSRole
                    {
                        Name = "SuperAdmin",
                        Description = "The real OG"
                    };
                    var bosa = new QMSRole
                    {
                        Name = "Bosa",
                        Description = "Bosa staff"
                    };
                    var fosa = new QMSRole
                    {
                        Name = "Fosa",
                        Description = "Fosa"
                    };

                    if (!ctx.Roles.Any())
                    {
                        roleMgr.CreateAsync(superAdmin).GetAwaiter().GetResult();
                        roleMgr.CreateAsync(bosa).GetAwaiter().GetResult();
                        roleMgr.CreateAsync(fosa).GetAwaiter().GetResult();
                    }

                    if (!ctx.Users.Any(u => u.UserName == "admin@nyatiqms.com"))
                    {
                        // Create a admin
                        var adminUser = new QMSUser
                        {
                            UserName = "admin@nyatiqms.com",
                            Email = "admin@nyatiqms.com",
                            FullName = "Nyati Admin",
                            EmailConfirmed = true
                        };
                        var result = userMgr.CreateAsync(adminUser, "nyatiQMS@2021!");

                        // Add a role
                        if (result.GetAwaiter().GetResult().Succeeded)
                        {
                            userMgr.AddToRoleAsync(adminUser, superAdmin.Name).GetAwaiter().GetResult();
                            userMgr.AddToRoleAsync(adminUser, bosa.Name).GetAwaiter().GetResult();
                            userMgr.AddToRoleAsync(adminUser, fosa.Name).GetAwaiter().GetResult();
                        }

                    }
                    DbInitializer.Initialize(context);

                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            host.Run();
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
