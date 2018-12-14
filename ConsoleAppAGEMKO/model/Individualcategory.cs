using System;
using System.Collections.Generic;

namespace ConsoleAppAGEMKO.model
{
    public partial class Individualcategory
    {
        public Individualcategory()
        {
            Businesses = new HashSet<Businesses>();
        }

        public int IndividualCategoryId { get; set; }
        public string IndividualCategoryDescr { get; set; }

        public virtual ICollection<Businesses> Businesses { get; set; }
    }
}
