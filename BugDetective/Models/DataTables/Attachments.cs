using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugDetective.Models.DataTables
{
    public class Attachments
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int? CommentId { get; set; }
        public string Filepath { get; set; }
        public DateTimeOffset Created { get; set; }
        public string UserId { get; set; }
        public string FileUrl { get; set; }

        public virtual Tickets Ticket { get; set; }
        public virtual Comments Comment { get; set; }
    }
}