using System;

namespace AdventureWorks.Physical
{
    public class StateProvidence
    {
        public string StateProvidenceCode{get;set;}
        public CountryRegion CountryRegion{get;set;}
        public string CountryRegionCode{get;set;}
        public string Name{get;set;}
        public Guid RowGuid{get;set;}
        public DateTimeOffset ModifiedDate{get;set;}
    }
}