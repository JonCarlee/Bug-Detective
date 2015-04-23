using BugDetective.Models.DataTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugDetective.Models
{
    public static class Extensions
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static void AddUser(this Projects project, string id)
        {
            project.Users.Add(new ApplicationUser());
            var newUser = new ApplicationUser{Id = id};
            db.Users.Attach(newUser);
            project.Users.Add(newUser);
        }

    }
}