using System;
using System.Collections.Generic;

namespace ConsoleAppAGEMKO.model
{
    public partial class Mainactivity
    {
        public Mainactivity()
        {
            Businesses = new HashSet<Businesses>();
        }

        public int MainActivityId { get; set; }
        public string MainActivityDescr { get; set; }

        public virtual ICollection<Businesses> Businesses { get; set; }
    }
}
