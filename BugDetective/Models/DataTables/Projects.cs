﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugDetective.Models.DataTables
{
    public class Projects
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ApplicationUser> Users { get; set; }
    }
}