using BugDetective.Models.LookupTables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugDetective.Models.DataTables
{
    public class Tickets
    {
        public Tickets()
        {
            this.TicketComments = new HashSet<Comments>();
            this.Attachments = new HashSet<Attachments>();
        }
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public int ProjectId { get; set; }
        public int TicketTypeId { get; set; }
        public int TicketPriorityId { get; set; }
        public int TicketStatusId { get; set; }
        public string OwnerUserId { get; set; }
        public string AssignedToUserId { get; set; }

        public virtual ICollection<Comments> TicketComments { get; set; }
        public virtual ICollection<Attachments> Attachments { get; set; }
        public virtual Projects Project { get; set; }
        public virtual TicketTypes TicketType { get; set; }
        public virtual TicketPriorities TicketPriority { get; set; }
        public virtual TicketStatuses TicketStatus { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        public virtual ApplicationUser Assigned { get; set; }
    }
}