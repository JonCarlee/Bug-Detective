using BugDetective.Models.DataTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugDetective.Models.LookupTables
{
    public class TicketStatuses
    {
        public int Id { get; set; }
        public string Name { get; set; }

        virtual public List<Tickets> Tickets { get; set; }

    }
}