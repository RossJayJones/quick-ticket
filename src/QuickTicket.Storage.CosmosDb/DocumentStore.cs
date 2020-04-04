using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace QuickTicket.Storage.CosmosDb
{
    public class DocumentStore : IDocumentStore
    {
        private readonly CosmosClient _client;
        private readonly string _databaseName;
        private readonly ContainerInfo _containerInfo;
        private Container _container;
        
        public DocumentStore(CosmosClient client,
            string databaseName,
            ContainerInfo containerInfo)
        {
            _client = client;
            _databaseName = databaseName;
            _containerInfo = containerInfo;
        }

        public async Task Init()
        {
            var databaseResponse = await _client.CreateDatabaseIfNotExistsAsync(_databaseName);
            var database = databaseResponse.Database;
            _container = await database.CreateContainerIfNotExistsAsync(_containerInfo.ContainerProperties,
                _containerInfo.Throughput,
                _containerInfo.ReadRequestOptions);
        }

        public IDocumentSession CreateSession()
        {
            if (_container == null)
            {
                throw new InvalidOperationException($"The document store has not been initialized.");
            }
            
            return new DocumentSession(_container, _containerInfo);
        }
    }
}