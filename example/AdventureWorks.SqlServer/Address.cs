using System;

namespace AdventureWorks.Physical
{
    public class Address
    {
        public int AddressId{get;set;}
        public string AddressLine1{get;set;}
        public string AddressLine2{get;set;}
        public string City{get;set;}
        public StateProvidence StateProvidence{get;set;}
        public int StateProvidenceID{get;set;}
        public string PostalCode{get;set;}
        public Guid RowGuid{get;set;}
        public DateTimeOffset ModifiedDate{get;set;}

        public AddressType AddressType{get;set;}
        public int AddressTypeID{get;set;}
    }
}