using System;
using System.Collections.Generic;

namespace ConsoleAppAGEMKO.model
{
    public partial class Registrytype
    {
        public Registrytype()
        {
            Businesses = new HashSet<Businesses>();
        }

        public int RegistryTypeId { get; set; }
        public string RegistryTypeDescr { get; set; }

        public virtual ICollection<Businesses> Businesses { get; set; }
    }
}
