using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace QuickTicket.Storage.CosmosDb
{
    public static class ServiceCollectionExtensions
    {
        public static async Task AddCosmosDbStorageAsync(IServiceCollection services,
            CosmosDbConfiguration configuration,
            IReadOnlyDictionary<Type, ContainerInfo> containerInfo,
            CosmosClientOptions options = null)
        {
            var client = new CosmosClient(configuration.ConnectionString, options ?? new CosmosClientOptions
            {
                ConnectionMode = ConnectionMode.Gateway,
                ConsistencyLevel = ConsistencyLevel.Session
            });
            var documentStoreFactory = new DocumentStoreFactory(client, containerInfo);
            var documentStore = await documentStoreFactory.Create(configuration.DatabaseName);
            services.AddSingleton(p => documentStore);
        }
    }
}