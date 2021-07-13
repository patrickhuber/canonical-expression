using AdventureWorks.Logical;

namespace AdventureWorks.Logical.PersonRead
{
    public class PersonReadResponse
    {
        public int? TotalCount { get; set; }
        public Person[] People{ get; set; }
    }
}