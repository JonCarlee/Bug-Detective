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
        public ActionResult Create([Bind(Include = "Id,Title,Description,Created,ProjectId,TicketTypeId,TicketStatusId,OwnerUserId")] Tickets ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.TicketStatusId = 1;
                ticket.TicketPriorityId = 4;
                ticket.Created = System.DateTimeOffset.Now;
                ticket.Owner = db.Users.Find(ticket.OwnerUserId);
                var test = new Tickets();
                test = ticket;
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
            return View(tickets);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedToUserId")] Tickets tickets)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                //AsNoTracking - Get values but don't reference object in database
                var OldTicket = (from t in db.Tickets.AsNoTracking()
                                 where t.Id == tickets.Id
                                 select t).FirstOrDefault();

                var EditId = Guid.NewGuid().ToString();
                var reassign = false;

                List<TicketHistory> History = new List<TicketHistory>();
                //OR
                //var oldTicket = db.Ticket.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);
                if (OldTicket.Description != tickets.Description)
                {
                    var DescriptionHistory = helper.MakeTicketHistory(EditId, tickets.Id, user.Id, "Description", OldTicket.Description, tickets.Description);
                    History.Add(DescriptionHistory);
                }

                if (OldTicket.ProjectId != tickets.ProjectId)
                {
                    var ProjectHistory = helper.MakeTicketHistory(EditId, tickets.Id, user.Id, "Project", OldTicket.Project.Name, tickets.Project.Name);
                    History.Add(ProjectHistory);
                }

                if (OldTicket.TicketTypeId != tickets.TicketTypeId)
                {
                    var TicketTypeHistory = helper.MakeTicketHistory(EditId, tickets.Id, user.Id, "Ticket Type", OldTicket.TicketType.Name, tickets.TicketType.Name);
                    History.Add(TicketTypeHistory);
                }

                if (OldTicket.TicketPriorityId != tickets.TicketPriorityId)
                {
                    var TicketPriorityHistory = helper.MakeTicketHistory(EditId, tickets.Id, user.Id, "Ticket Priority", OldTicket.TicketPriority.Name, tickets.TicketPriority.Name);
                    History.Add(TicketPriorityHistory);
                }

                if (OldTicket.TicketStatusId != tickets.TicketStatusId)
                {
                    var TicketStatusHistory = helper.MakeTicketHistory(EditId, tickets.Id, user.Id, "Ticket Status", OldTicket.TicketStatus.Name, tickets.TicketStatus.Name);
                    History.Add(TicketStatusHistory);
                }

                if (OldTicket.AssignedToUserId != tickets.AssignedToUserId)
                {
                    var AssignedHistory = helper.MakeTicketHistory(EditId, tickets.Id, user.Id, "Assigned User", OldTicket.AssignedToUserId, tickets.AssignedToUserId);
                    History.Add(AssignedHistory);
                    reassign = true;
                }

                db.TicketHistories.AddRange(History);
                if (reassign == true) { 
                new EmailService().SendAsync(new IdentityMessage{
                    Subject = "You have been assigned a new ticket",
                    Destination = user.Email,
                    Body = "This is to make sure it gets there"
                });
                }
                tickets.Updated = DateTimeOffset.Now;
                db.Entry(tickets).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", tickets.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", tickets.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", tickets.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", tickets.TicketTypeId);
            return View(tickets);
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
