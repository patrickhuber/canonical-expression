using System.Threading.Tasks;

namespace AdventureWorks.Logical.PersonDelete
{
    public interface IPersonDelete
    {
        Task<PersonDeleteResponse> DeleteAsync(PersonDeleteRequest request);
    }
}