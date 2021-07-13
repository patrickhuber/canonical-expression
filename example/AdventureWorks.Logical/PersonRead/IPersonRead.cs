using System.Threading.Tasks;

namespace AdventureWorks.Logical.PersonRead
{
    public interface IPersonRead
    {
        Task<PersonReadResponse> ReadAsync(PersonReadRequest request);
    }
}