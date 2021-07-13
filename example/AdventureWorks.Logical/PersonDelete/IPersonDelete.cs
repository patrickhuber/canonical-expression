namespace AdventureWorks.Logical.PersonDelete
{
    public interface IPersonDelete
    {
        PersonDeleteResponse Read(PersonDeleteRequest request);
    }
}