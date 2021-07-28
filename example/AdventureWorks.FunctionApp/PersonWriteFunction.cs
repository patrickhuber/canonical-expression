using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Dapr.AzureFunctions.Extension;
using AdventureWorks.Logical.PersonWrite;

namespace AdventureWorks.FunctionApp
{
    public class PersonWriteFunction
    {
        private IPersonWrite _writeRequest;
        private readonly ILogger _log;

        public PersonWriteFunction(IPersonWrite writeRequest,
            ILogger log)
        {
            _writeRequest = writeRequest;
            _log = log;
        }

        [FunctionName("PersonWrite")]
        public async Task<PersonWriteResponse> Run(
            [DaprServiceInvocationTrigger]PersonWriteRequest writeRequest)
        {
            _log.LogInformation("PersonWrite function exectued");
            return await _writeRequest.WriteAsync(writeRequest);
        }
    }
}
