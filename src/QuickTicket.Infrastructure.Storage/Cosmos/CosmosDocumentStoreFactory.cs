using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace QuickTicket.Infrastructure.Storage.Cosmos
{
    public class CosmosDocumentStoreFactory : IDocumentStoreFactory
    {
        private readonly CosmosClient _client;
        private readonly IReadOnlyDictionary<Type, CosmosContainerInfo> _containers;
        
        public CosmosDocumentStoreFactory(CosmosClient client,
            IReadOnlyDictionary<Type, CosmosContainerInfo> containers)
        {
            _client = client;
            _containers = containers;
        }

        public async Task<IDocumentStore> Create(string databaseName)
        {
            var databaseResponse = await _client.CreateDatabaseIfNotExistsAsync(databaseName);
            var database = databaseResponse.Database;
            var containers = new Dictionary<Type, (Container, CosmosContainerInfo)>();
            foreach (var pair in _containers)
            {
                var containerResponse = await database.CreateContainerIfNotExistsAsync(pair.Value.ContainerProperties,
                    pair.Value.Throughput,
                    pair.Value.ReadRequestOptions);
                containers.Add(pair.Key, (containerResponse.Container, pair.Value));
            }
            var documentStore = new CosmosDocumentStore(containers);
            return documentStore;
        }
    }
}