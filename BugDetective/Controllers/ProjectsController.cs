using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugDetective.Models;
using BugDetective.Models.DataTables;

namespace BugDetective.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        public ActionResult Index()
        {
            return View(db.Projects.ToList());
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects projects = db.Projects.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }
            return View(projects);
        }

        // GET: Projects/Create
        [Authorize(Roles="Admin")]
        public ActionResult Create()
        {
            var allUsers = db.Users.ToList();
            var users = new List<ApplicationUser>();
            
           // var managers = db.Users.Where(u => u.Roles.Any(r => r.RoleId.Equals(manager.;
            foreach (var user in allUsers)
            {
                foreach(var userroles in user.Roles)
                {
                    foreach (var role in db.Roles.ToList())
                    {
                        if (userroles.RoleId == role.Id && role.Name == "Project Manager")
                            users.Add(user);
                    }
                }
                    
            }
            ViewBag.Managers = new SelectList(users, "Id", "Email");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Manager")] Projects project, string Managers)
        {
            if (ModelState.IsValid)
            {
                /*
                var user = db.Users.Find(Managers);
                user.Projects.Add(project);
                project.Manager = user;
                project.Users.Add(user);
                db.Projects.Add(project);
                */
                var user = db.Users.Find(Managers);
                project.ManagerId = user.Id; 
                db.Projects.Add(project);
                project.Users.Add(user);
                db.SaveChanges();

                //foreach(UserId in UserList)
                    //project.Users.Add(UserId)

                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects projects = db.Projects.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }
            return View(projects);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Projects projects, string id)
        {
            if (ModelState.IsValid)
            {
                db.Entry(projects).State = EntityState.Modified;
                db.SaveChanges();
                projects.Users.Add(new ApplicationUser());
                var newUser = new ApplicationUser { Id = id };
                db.Users.Attach(newUser);
                projects.Users.Add(newUser);
                return RedirectToAction("Index");
            }
            return View(projects);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects projects = db.Projects.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }
            return View(projects);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Projects projects = db.Projects.Find(id);
            db.Projects.Remove(projects);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
