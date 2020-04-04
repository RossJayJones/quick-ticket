using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace QuickTicket.Storage.CosmosDb
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCosmosDbStorageAsync(IServiceCollection services,
            CosmosDbConfiguration configuration,
            ContainerInfo containerInfo,
            CosmosClientOptions options = null)
        {
            var client = new CosmosClient(configuration.ConnectionString, options ?? new CosmosClientOptions
            {
                ConnectionMode = ConnectionMode.Gateway,
                ConsistencyLevel = ConsistencyLevel.Session
            });
            var documentStore = new DocumentStore(client,
                configuration.DatabaseName,
                containerInfo);
            services.AddSingleton(p => documentStore);
        }
    }
}