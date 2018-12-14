using System;
using System.Collections.Generic;

namespace ConsoleAppAGEMKO.model
{
    public partial class Status
    {
        public Status()
        {
            Businesses = new HashSet<Businesses>();
        }

        public int StatusId { get; set; }
        public string StatusDescr { get; set; }

        public virtual ICollection<Businesses> Businesses { get; set; }
    }
}
