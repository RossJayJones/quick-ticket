using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace QuickTicket.Storage.CosmosDb
{
    public class DocumentStoreFactory : IDocumentStoreFactory
    {
        private readonly CosmosClient _client;
        private readonly IReadOnlyDictionary<Type, ContainerInfo> _containers;
        
        public DocumentStoreFactory(CosmosClient client,
            IReadOnlyDictionary<Type, ContainerInfo> containers)
        {
            _client = client;
            _containers = containers;
        }

        public async Task<IDocumentStore> Create(string databaseName)
        {
            var databaseResponse = await _client.CreateDatabaseIfNotExistsAsync(databaseName);
            var database = databaseResponse.Database;
            var containers = new Dictionary<Type, (Container, ContainerInfo)>();
            foreach (var pair in _containers)
            {
                var containerResponse = await database.CreateContainerIfNotExistsAsync(pair.Value.ContainerProperties,
                    pair.Value.Throughput,
                    pair.Value.ReadRequestOptions);
                containers.Add(pair.Key, (containerResponse.Container, pair.Value));
            }
            var documentStore = new DocumentStore(containers);
            return documentStore;
        }
    }
}