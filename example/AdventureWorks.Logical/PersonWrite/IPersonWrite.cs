using System.Threading.Tasks;

namespace AdventureWorks.Logical.PersonWrite
{
    public interface IPersonWrite
    {
        Task<PersonWriteResponse> WriteAsync(PersonWriteRequest request);
    }
}