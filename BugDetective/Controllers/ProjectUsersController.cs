using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BugDetective.Models;

namespace BugDetective.Controllers
{
    public class ProjectUsersController : Controller
    {
        private UserProjectsHelper helper = new UserProjectsHelper();
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ProjectUsers
	    public ActionResult AssignUsers(int id)
    	{
	    	var model = new ProjectUsersViewModel();
	    	var project = db.Projects.Find(id);
	    	model.projectId = project.Id;
	    	model.projectName = project.Name;
	    	model.Users = new MultiSelectList(helper.ListUsersNotOnProject(id).OrderBy(u => u.DisplayName), "Id", "DisplayName");
	    	return View(model);
	    }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult AssignUsers(ProjectUsersViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.SelectedUsers != null)
                {
                    foreach (string id in model.SelectedUsers)
                    {
                        helper.AddUserToProject(id, model.projectId);
                    }
                    return RedirectToAction("Index", "Projects");
                }
                else
                {
                    ViewBag.Error = "Please select a user.";
                }
            }
            return View(model);
        }

        public ActionResult RemoveUsers(int id)
        {
            var model = new ProjectUsersViewModel();
            var project = db.Projects.Find(id);
            model.projectId = project.Id;
            model.projectName = project.Name;
            model.Users = new MultiSelectList(helper.ListUsersOnProject(id).OrderBy(u => u.DisplayName), "Id", "DisplayName");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult RemoveUsers(ProjectUsersViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.SelectedUsers != null)
                {
                    foreach (string id in model.SelectedUsers)
                    {
                        helper.RemoveUserFromProject(id, model.projectId);
                    }
                    return RedirectToAction("Index", "Projects");
                }
                else
                {
                    ViewBag.Error = "Please select a user.";
                }
            }
            return View(model);
        }

    }
}