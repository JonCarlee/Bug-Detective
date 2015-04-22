using BugDetective.Models;
using BugDetective.Models.LookupTables;
using BugDetective.Models.DataTables;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace BugDetective.Migrations
{

    internal sealed class Configuration : DbMigrationsConfiguration<BugDetective.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BugDetective.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }
            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }
            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));
            /*
            ApplicationUser user;

            if (!context.Users.Any(r => r.Email == "joncarlee@gmail.com"))
            {
                user = new ApplicationUser
                {
                    UserName = "Jon Carlee",
                    Email = "joncarlee@gmail.com",
                };
                userManager.Create(new ApplicationUser { }, "Abc123");
            }
            else
            {
                user = context.Users.Single(u => u.Email == "joncarlee@gmail.com");
            }
            if (!userManager.IsInRole(user.Id, "Admin"))
            {
                userManager.AddToRole(user.Id, "Admin");
            }
            */
            //Adding Other Things To Database
            
            new TicketStatuses {Name = "New"};
            new TicketStatuses {Name = "Opened"};
            new TicketStatuses {Name = "Closed"};
            new TicketStatuses {Name = "Resolved"};

            new TicketPriorities {Name = "Low"};
            new TicketPriorities {Name = "High"};
            new TicketPriorities {Name = "Critical"};

            new TicketTypes { Name = "Add" };
            new TicketTypes { Name = "Change" };
            new TicketTypes { Name = "Problem" };

            }
        }
    }

