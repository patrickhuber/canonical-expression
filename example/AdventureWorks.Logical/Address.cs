using System;

namespace AdventureWorks.Logical
{
    public class Address
    {
        public int Id{get;set;}
        public string AddressLine1{get;set;}
        public string AddressLine2{get;set;}
        public string City{get;set;}
        public StateOrProvidence StateOrProvidence{get;set;}
        public string PostalCode{get;set;}
        public DateTimeOffset ModifiedDate{get;set;}
    }
}