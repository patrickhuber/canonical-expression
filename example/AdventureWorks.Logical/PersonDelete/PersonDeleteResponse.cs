using System;

namespace AdventureWorks.Logical.PersonDelete
{
    public class PersonDeleteResponse
    {
        public PersonDeleteResult[] People { get; set; }
    }

    public class PersonDeleteResult
    { 
        public Guid Id { get; set; }
    }
}