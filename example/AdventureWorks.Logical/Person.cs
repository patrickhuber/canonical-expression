using System;
using System.Collections.Generic;

namespace AdventureWorks.Logical
{
    public class Person
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public IReadOnlyList<EmailAddress> EmailAddresses{get;set;}
        public IReadOnlyList<Phone> PhoneNumbers{get;set;}
        public string AdditionalContactInfo { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
    }
}