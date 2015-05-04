using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using BugDetective.Models.DataTables;

namespace BugDetective.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public List<Projects> Projects { get; set; }
        public List<Tickets> Tickets { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<BugDetective.Models.DataTables.Attachments> Attachments { get; set; }

        public System.Data.Entity.DbSet<BugDetective.Models.DataTables.Comments> Comments { get; set; }
        
        public System.Data.Entity.DbSet<BugDetective.Models.DataTables.Projects> Projects { get; set; }
        
        public System.Data.Entity.DbSet<BugDetective.Models.DataTables.Tickets> Tickets { get; set; }
        
        public System.Data.Entity.DbSet<BugDetective.Models.LookupTables.TicketPriorities> TicketPriorities { get; set; }

        public System.Data.Entity.DbSet<BugDetective.Models.LookupTables.TicketStatuses> TicketStatuses { get; set; }

        public System.Data.Entity.DbSet<BugDetective.Models.LookupTables.TicketTypes> TicketTypes { get; set; }
    }
}