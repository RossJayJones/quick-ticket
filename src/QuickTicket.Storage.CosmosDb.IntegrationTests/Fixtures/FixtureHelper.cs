using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace QuickTicket.Storage.CosmosDb.IntegrationTests.Fixtures
{
    public static class FixtureHelper
    {
        public static CosmosDbConfiguration GetConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<DocumentStoreTests>()
                .Build();
            return configuration.GetSection(CosmosDbConfiguration.SectionName).Get<CosmosDbConfiguration>();
        }

        public static async Task<IDocumentStore> CreateDocumentStore(IReadOnlyDictionary<Type, ContainerInfo> containerInfo = null)
        {
            var configuration = FixtureHelper.GetConfiguration();
            var client = new CosmosClient(configuration.ConnectionString);
            var documentStoreFactory = new DocumentStoreFactory(client, containerInfo ?? new Dictionary<Type, ContainerInfo>());
            var documentStore = await documentStoreFactory.Create(configuration.DatabaseName);
            return documentStore;
        }

        public static ContainerInfo<TestDocument> CreateTestContainerInfo()
        {
            return new ContainerInfo<TestDocument>(
                getDocumentPartitionKey: doc => new PartitionKey(doc.PartitionKey),
                getDocumentId: doc => doc.Id,
                getDocumentEtag: doc => doc.Etag,
                containerProperties: new ContainerProperties
                {
                    Id = "IntegrationTests",
                    DefaultTimeToLive = 120,
                    PartitionKeyPath = "/PartitionKey",
                },
                throughput: 400);
        }
    }
}