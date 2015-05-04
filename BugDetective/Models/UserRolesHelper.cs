using BugDetective.Models.DataTables;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BugDetective.Models
{

    public class UserProjectsHelper
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        public bool IsUserInProject(string userId, int projectId)
        {
            return db.Projects.Find(projectId).Users.Any(u => u.Id == userId);
        }

        public void AddUserToProject(string userId, int projectId){
	    if (!IsUserInProject(userId, projectId))
	    {
    	var project = db.Projects.Find(projectId);
	    project.Users.Add(db.Users.Find(userId));
    	db.Entry(project).State = EntityState.Modified;
	    db.SaveChanges();
	    }
	    }

	    public void RemoveUserFromProject (string userId, int projectId){
	    if (IsUserInProject(userId, projectId))
	{
	/*My one liner
	db.Projects.Find(projectId).Users.Remove(db.Users.Find(userId));
	*/
	    var project = db.Projects.Find(projectId);
	    project.Users.Remove(db.Users.Find(userId));
	    db.Entry(project).State = EntityState.Modified;
	    db.SaveChanges();
	    }
	    }

	public ICollection<Projects> ListProjectsForUser(string userId){
		return db.Users.Find(userId).Projects;
	}

	public ICollection<ApplicationUser> ListUsersOnProject(int projectId){
		return db.Projects.Find(projectId).Users;
	}

	public ICollection<ApplicationUser> ListUsersNotOnProject (int projectId){

	return db.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToList();

	}
}

    public class ProjectUsersViewModel{
	    public int projectId { get; set; }
	    public string projectName { get; set; }
    	[Display(Name = "Available Users")]
    	public System.Web.Mvc.MultiSelectList Users { get; set; }
	    public string[] SelectedUsers { get; set; }
    }
        public class UserRolesHelper
        {
            private UserManager<ApplicationUser> manager =
        new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(
                new ApplicationDbContext()));
            // GET: Helper

            public class UserRolesViewModel
            {
                //This will need to be tweaked
                public string roleName { get; set; }
                public ApplicationUser User { get; set; }
            }

            public class ListUserRole
            {
                public List<ApplicationUser> Users { get; set; }
                public List<Microsoft.AspNet.Identity.EntityFramework.IdentityRole> Roles { get; set; }
            }





            public bool IsUserInRole(string userId, string roleName)
            {
                return manager.IsInRole(userId, roleName);
            }

            public IList<string> ListUserRoles(string userId)
            {
                return manager.GetRoles(userId);
            }

            public bool AddUserToRole(string userId, string roleName)
            {
                var result = manager.AddToRole(userId, roleName);
                return result.Succeeded;
            }

            public bool RemoveUserFromRole(string userId, string roleName)
            {
                var result = manager.RemoveFromRole(userId, roleName);
                return result.Succeeded;
            }

            public IList<ApplicationUser> UsersInRole(string roleName)
            {
                var db = new ApplicationDbContext();
                var resultList = new List<ApplicationUser>();

                //This is INCREDIBLY inefficent.  Need to find a call that hits the database once.
                foreach (var user in db.Users)
                {
                    if (IsUserInRole(user.Id, roleName))
                    {
                        resultList.Add(user);
                    }
                }
                return resultList;
            }

            public IList<ApplicationUser> UsersNotInRole(string roleName)
            {
                var resultList = new List<ApplicationUser>();
                //Also INCREDIBLY inefficent.  Same as above.
                foreach (var user in manager.Users)
                {
                    if (!IsUserInRole(user.Id, roleName))
                    {
                        resultList.Add(user);
                    }
                }
                return resultList;
            }
        }
    }
