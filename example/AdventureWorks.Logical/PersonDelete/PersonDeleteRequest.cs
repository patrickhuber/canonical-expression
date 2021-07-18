using System;

namespace AdventureWorks.Logical.PersonDelete
{
    public class PersonDeleteRequest
    {
        public PersonDeleteCriteria[] Where { get; set; }

        public PersonDeleteRequest(params PersonDeleteCriteria[] where) => Where = where;
    }

    public class PersonDeleteCriteria
    {
        public Guid Id { get; set; }

        public PersonDeleteCriteria() { }
        public PersonDeleteCriteria(Guid id) => Id = id;
    }
}