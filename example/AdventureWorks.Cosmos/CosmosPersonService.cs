using AdventureWorks.Logical.PersonRead;
using AdventureWorks.Logical.PersonWrite;
using System;
using System.Threading.Tasks;

namespace AdventureWorks.Cosmos
{
    public class CosmosPersonService : IPersonRead, IPersonWrite
    {
        public Task<PersonReadResponse> ReadAsync(PersonReadRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<PersonWriteResponse> WriteAsync(PersonWriteRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
