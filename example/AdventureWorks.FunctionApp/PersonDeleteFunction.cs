using AdventureWorks.Logical.PersonDelete;
using Dapr.AzureFunctions.Extension;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace AdventureWorks.FunctionApp
{
    public class PersonDeleteFunction
    {
        IPersonDelete _personDelete;
        ILogger _log;

        public PersonDeleteFunction(IPersonDelete personDelete, ILogger log)
        {
            _personDelete = personDelete;
            _log = log;
        }


        [FunctionName("PersonDelete")]
        public async Task<PersonDeleteResponse> Run(
            [DaprServiceInvocationTrigger] PersonDeleteRequest request)
        {
            _log.LogInformation("PersonDelete function exectued");
            return await _personDelete.DeleteAsync(request);
        }
    }
}
