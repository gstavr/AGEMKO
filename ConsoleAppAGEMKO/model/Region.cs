using System;
using System.Collections.Generic;

namespace ConsoleAppAGEMKO.model
{
    public partial class Region
    {
        public Region()
        {
            Businesses = new HashSet<Businesses>();
            Regionalunity = new HashSet<Regionalunity>();
        }

        public int RegionId { get; set; }
        public string RegionDescr { get; set; }

        public virtual ICollection<Businesses> Businesses { get; set; }
        public virtual ICollection<Regionalunity> Regionalunity { get; set; }
    }
}
