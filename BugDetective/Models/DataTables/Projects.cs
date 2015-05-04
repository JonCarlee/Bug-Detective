using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugDetective.Models.DataTables
{
    public class Projects
    {
        public Projects(){
            this.Users = new HashSet<ApplicationUser>();
            this.Tickets = new HashSet<Tickets>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ManagerId { get; set; }

        virtual public ApplicationUser Manager { get; set; }
        virtual public ICollection<ApplicationUser> Users { get; set; }
        virtual public ICollection<Tickets> Tickets { get; set; }
    }
}