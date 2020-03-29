using Microsoft.Azure.Cosmos;

namespace QuickTicket.Storage.CosmosDb
{
    public class DocumentMetadata
    {
        public DocumentMetadata(string id,
            PartitionKey partitionKey,
            string etag)
        {
            Id = id;
            PartitionKey = partitionKey;
            Etag = etag;
        }
        
        public string Id { get; }

        public PartitionKey PartitionKey { get; }
        
        public string Etag { get; }
    }
}