using System;
using System.Collections.Generic;

namespace AdventureWorks.Physical
{
    public class Person
    {
        public int Id { get; set; }
        public string NameStyle { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string EmailPromotion { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string AdditionalContactInfo { get; set; }
        public Guid RowGuid { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }

        public IList<Phone> PhoneNumbers{get;set;}
        public IList<EmailAddress> EmailAddresses{get;set;}
    }
}