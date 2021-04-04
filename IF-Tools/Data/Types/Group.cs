﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IFTools.Data.Types
{
    public class Group
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }

        public Group(string id, string name)
        {
            Id = new Guid(id);
            Name = name;
        }
    }
}
