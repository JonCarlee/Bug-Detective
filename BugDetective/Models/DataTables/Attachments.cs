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
        public string Filepath { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Created { get; set; }
        public string UserId { get; set; }
        public string FileUrl { get; set; }
    }
}