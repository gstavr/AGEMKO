using System;
using System.Collections.Generic;

namespace ConsoleAppAGEMKO.model
{
    public partial class Representative
    {
        public Representative()
        {
            Businesses = new HashSet<Businesses>();
        }

        public int RepresentativeId { get; set; }
        public string RepresentativeFullName { get; set; }

        public virtual ICollection<Businesses> Businesses { get; set; }
    }
}
