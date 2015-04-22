using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugDetective.Models
{
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
            public string roleId { get; set; }
            public string roleName { get; set; }
            public List<ApplicationUser> Users { get; set; }
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