using System;
using System.Collections.Generic;

namespace ConsoleAppAGEMKO.model
{
    public partial class Businesses
    {
        public int BusinessesId { get; set; }
        public string BusinessesAgemko { get; set; }
        public string BusinessesAmke { get; set; }
        public string BusinessesVat { get; set; }
        public string BusinessesDescr { get; set; }
        public string BusinessesDistinctTitle { get; set; }
        public int? BusinessesNumMembers { get; set; }
        public string BusinessesAddress { get; set; }
        public string BusinessesEmail { get; set; }
        public DateTime? BusinessesRegisterDate { get; set; }
        public DateTime? BusinessesReviewDate { get; set; }
        public int? MainActivityMainActivityId { get; set; }
        public int? RegistryTypeRegistryTypeId { get; set; }
        public int? IndividualCategoryIndividualCategoryId { get; set; }
        public int? StatusStatusId { get; set; }
        public int? RegionRegionId { get; set; }
        public int? RegionalUnityRegionalUnityId { get; set; }
        public int? MunicipalityMunicipalityId { get; set; }
        public int? RepresentativeRepresentativeId { get; set; }
        public string BusinessesExtraField { get; set; }

        public virtual Individualcategory IndividualCategoryIndividualCategory { get; set; }
        public virtual Mainactivity MainActivityMainActivity { get; set; }
        public virtual Municipality MunicipalityMunicipality { get; set; }
        public virtual Region RegionRegion { get; set; }
        public virtual Regionalunity RegionalUnityRegionalUnity { get; set; }
        public virtual Registrytype RegistryTypeRegistryType { get; set; }
        public virtual Representative RepresentativeRepresentative { get; set; }
        public virtual Status StatusStatus { get; set; }
    }
}
