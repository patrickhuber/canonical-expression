using AdventureWorks.Cosmos;
using AdventureWorks.Logical.PersonDelete;
using AdventureWorks.Logical.PersonRead;
using AdventureWorks.Logical.PersonWrite;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(AdventureWorks.FunctionApp.Startup))]

namespace AdventureWorks.FunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton(services => 
            {
                var cosmosClient = new CosmosClient("Cosmos:ConnectionString");
                return new CosmosPersonService(cosmosClient);
            });

            builder.Services.AddSingleton<IPersonRead, CosmosPersonService>();
            builder.Services.AddSingleton<IPersonWrite, CosmosPersonService>();
            builder.Services.AddSingleton<IPersonDelete, CosmosPersonService>();
        }
    }
}
