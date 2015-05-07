using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugDetective.Models.DataTables
{
    public class Comments
    {
        public Comments()
        {
            this.Attachments = new HashSet<Attachments>();
        }
        public int Id { get; set; }
        [Required]
        public string Comment { get; set; }
        public DateTimeOffset Created { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }

        public virtual Tickets Ticket { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Attachments> Attachments { get; set; }
    }
}