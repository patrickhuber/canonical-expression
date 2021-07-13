using System;

namespace AdventureWorks.Physical
{
    public class AddressType
    {
        public int AddressTypeID{get;set;}
        public string Name{get;set;}
        public DateTimeOffset ModifiedDate{get;set;}
        public Guid RowGuid{get;set;}
    }
}