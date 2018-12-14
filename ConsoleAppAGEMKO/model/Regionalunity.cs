using System;
using System.Collections.Generic;

namespace ConsoleAppAGEMKO.model
{
    public partial class Regionalunity
    {
        public Regionalunity()
        {
            Businesses = new HashSet<Businesses>();
            Municipality = new HashSet<Municipality>();
        }

        public int RegionalUnityId { get; set; }
        public string RegionalUnityDescr { get; set; }
        public int RegionRegionId { get; set; }

        public virtual Region RegionRegion { get; set; }
        public virtual ICollection<Businesses> Businesses { get; set; }
        public virtual ICollection<Municipality> Municipality { get; set; }
    }
}
