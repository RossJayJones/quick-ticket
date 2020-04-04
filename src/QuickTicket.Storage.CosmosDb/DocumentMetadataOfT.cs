using Microsoft.Azure.Cosmos;

namespace QuickTicket.Storage.CosmosDb
{
    public class DocumentMetadata<TDocument> : DocumentMetadata
    {
        public DocumentMetadata(string id,
            PartitionKey partitionKey,
            string etag) : base(typeof(TDocument), id, partitionKey, etag)
        {
            
        }
    }
}