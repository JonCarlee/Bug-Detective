using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugDetective.Models;
using PagedList;
using PagedList.Mvc;
using Microsoft.AspNet.Identity;
using System.IO;
namespace BugDetective.Models.DataTables
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private TicketHistoriesHelper helper = new TicketHistoriesHelper();

        // GET: Tickets
        public ActionResult Index(string keyword, string column, int? page)
        {

            return View(db.Tickets.ToList());
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets tickets = db.Tickets.Find(id);
            if (tickets == null)
            {
                return HttpNotFound();
            }
            return View(tickets);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(string comment, HttpPostedFileBase[] images, int ticketId)
        {
            if (comment == "")
            {
                return RedirectToAction("Details", new { Id = ticketId });
            }
            var user = User.Identity.GetUserId();
            var aComment = new Comments
            {
                Comment = comment,
                Created = DateTimeOffset.Now,
                TicketId = ticketId,
                UserId = user,
            };
            var curr = new Attachments();
            var Attach = new List<Attachments>();
            if (images != null)
            {
                foreach (HttpPostedFileBase image in images)
                {
                    var ext = Path.GetExtension(image.FileName);
                    if (ext != ".png" && ext != ".jpg" && ext != ".txt")
                        ModelState.AddModelError("image", "Invalid Format");
                    if (ModelState.IsValid)
                    {
                        var filePath = "/Uploads/TicketAttachments/images/";
                        var absPath = Server.MapPath("~" + filePath);
                        image.SaveAs(Path.Combine(absPath, image.FileName));

                        curr = new Attachments
                        {
                            TicketId = ticketId,
                            CommentId = aComment.Id,
                            Filepath = filePath,
                            Created = aComment.Created,
                            UserId = aComment.UserId,
                            FileUrl = filePath + image.FileName
                        };
                        Attach.Add(curr);
                    }
                }
                aComment.Attachments = Attach;
                db.Comments.Add(aComment);
                db.Attachments.AddRange(Attach);
                db.Tickets.Find(ticketId).TicketComments.Add(aComment);
                db.SaveChanges();
                return RedirectToAction("Details", new { Id = ticketId });
            }
            else
            {
                db.Comments.Add(aComment);
                db.Tickets.Find(ticketId).TicketComments.Add(aComment);
                db.SaveChanges();
                return RedirectToAction("Details", new { Id = ticketId });
            }

        }

        // GET: Tickets/Create
        public ActionResult Create()
        {
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Created,ProjectId,TicketTypeId,TicketStatusId,OwnerId")] Tickets ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.TicketStatusId = 1;
                ticket.TicketPriorityId = 4;
                ticket.Created = System.DateTimeOffset.Now;
                ticket.Owner = db.Users.Find(ticket.OwnerId);
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets tickets = db.Tickets.Find(id);
            if (tickets == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", tickets.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", tickets.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", tickets.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", tickets.TicketTypeId);
            ViewBag.AssignedId = new SelectList(db.Users, "Id", "DisplayName");
            return View(tickets);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerId,AssignedId")] Tickets ticket)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                var project = db.Projects.Find(ticket.ProjectId);
                var type = db.TicketTypes.Find(ticket.TicketTypeId);
                var status = db.TicketStatuses.Find(ticket.TicketStatusId);
                var priority = db.TicketPriorities.Find(ticket.TicketPriorityId);
                var assigned = db.Users.Find(ticket.AssignedId);
                //AsNoTracking - Get values but don't reference object in database
                var OldTicket = (from t in db.Tickets.AsNoTracking()
                                 where t.Id == ticket.Id
                                 select t).FirstOrDefault();

                var EditId = Guid.NewGuid().ToString();
                var reassign = false;

                List<TicketHistory> History = new List<TicketHistory>();
                //OR
                //var oldTicket = db.Ticket.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);
                if (OldTicket.Description != ticket.Description)
                {
                    var DescriptionHistory = helper.MakeTicketHistory(EditId, ticket.Id, user.Id, "Description", OldTicket.Description, ticket.Description);
                    History.Add(DescriptionHistory);
                }

                if (OldTicket.ProjectId != ticket.ProjectId)
                {
                    var ProjectHistory = helper.MakeTicketHistory(EditId, ticket.Id, user.Id, "Project", OldTicket.Project.Name, project.Name);
                    History.Add(ProjectHistory);
                }

                if (OldTicket.TicketTypeId != ticket.TicketTypeId)
                {
                    var TicketTypeHistory = helper.MakeTicketHistory(EditId, ticket.Id, user.Id, "Ticket Type", OldTicket.TicketType.Name, type.Name);
                    History.Add(TicketTypeHistory);
                }

                if (OldTicket.TicketPriorityId != ticket.TicketPriorityId)
                {
                    var TicketPriorityHistory = helper.MakeTicketHistory(EditId, ticket.Id, user.Id, "Ticket Priority", OldTicket.TicketPriority.Name, priority.Name);
                    History.Add(TicketPriorityHistory);
                }

                if (OldTicket.TicketStatusId != ticket.TicketStatusId)
                {
                    var TicketStatusHistory = helper.MakeTicketHistory(EditId, ticket.Id, user.Id, "Ticket Status", OldTicket.TicketStatus.Name, status.Name);
                    History.Add(TicketStatusHistory);
                }

                if (OldTicket.AssignedId != ticket.AssignedId)
                {
                    var AssignedHistory = helper.MakeTicketHistory(EditId, ticket.Id, user.Id, "Assigned User", (OldTicket.Assigned != null ? OldTicket.Assigned.DisplayName : "null"), assigned.DisplayName);
                    History.Add(AssignedHistory);
                    reassign = true;
                }

                db.TicketHistories.AddRange(History);
                ticket.TicketHistories = History;
                if (reassign == true) { 
                new EmailService().SendAsync(new IdentityMessage{
                    Subject = "You have been assigned a new ticket",
                    Destination = user.Email,
                    Body = "This is to make sure it gets there"
                });
                }

                ticket.Updated = DateTimeOffset.Now;
                ticket.Assigned = db.Users.Find(ticket.AssignedId);
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets tickets = db.Tickets.Find(id);
            if (tickets == null)
            {
                return HttpNotFound();
            }
            return View(tickets);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tickets tickets = db.Tickets.Find(id);
            db.Tickets.Remove(tickets);
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
