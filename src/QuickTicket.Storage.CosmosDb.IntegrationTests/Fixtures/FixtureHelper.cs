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

        public static async Task<IDocumentStore> CreateDocumentStore(ContainerInfo containerInfo = null)
        {
            var configuration = FixtureHelper.GetConfiguration();
            var client = new CosmosClient(configuration.ConnectionString);
            var documentStore = new DocumentStore(client, configuration.DatabaseName, containerInfo ?? CreateTestContainerInfo());
            await documentStore.Init();
            return documentStore;
        }

        public static ContainerInfo CreateTestContainerInfo()
        {
            return new ContainerInfo(new ContainerProperties
                {
                    Id = "IntegrationTests",
                    DefaultTimeToLive = 120,
                    PartitionKeyPath = "/PartitionKey",
                },
                throughput: 400)
                .WithDocumentMetadataFactory<TestDocument>(doc => new DocumentMetadata<TestDocument>(doc.Id, new PartitionKey(doc.PartitionKey), doc.Etag));
        }
    }
}