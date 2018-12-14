using System;
using System.Collections.Generic;

namespace ConsoleAppAGEMKO.model
{
    public partial class Municipality
    {
        public Municipality()
        {
            Businesses = new HashSet<Businesses>();
        }

        public int MunicipalityId { get; set; }
        public string MunicipalityDescr { get; set; }
        public int RegionalUnityRegionalUnityId { get; set; }

        public virtual Regionalunity RegionalUnityRegionalUnity { get; set; }
        public virtual ICollection<Businesses> Businesses { get; set; }
    }
}
