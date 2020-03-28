using Newtonsoft.Json;

namespace QuickTicket.Storage.CosmosDb.IntegrationTests.Fixtures
{
    public class TestDocument
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("_etag")]
        public string Etag { get; set; }

        public string PartitionKey { get; set; }
    }
}