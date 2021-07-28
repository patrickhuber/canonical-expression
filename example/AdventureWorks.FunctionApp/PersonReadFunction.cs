using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Dapr.AzureFunctions.Extension;
using AdventureWorks.Logical.PersonRead;

namespace AdventureWorks.FunctionApp
{
    public class PersonReadFunction
    {
        private IPersonRead _personRead;
        private ILogger _log;

        public PersonReadFunction(IPersonRead personRead, ILogger log)
        {
            _personRead = personRead;
            _log = log;
        }

        [FunctionName("PersonRead")]
        public async Task<PersonReadResponse> Run(
            [DaprServiceInvocationTrigger]PersonReadRequest readRequest)
        {
            _log.LogInformation("PersonRead function exectued");
            return await _personRead.ReadAsync(readRequest);
        }
    }
}
