﻿using BugDetective.Models.DataTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugDetective.Models.LookupTables
{
    public class TicketTypes
    {
        public int Id { get; set; }
        public string Name { get; set; }

        virtual public List<Tickets> Tickets { get; set; }
    }
}