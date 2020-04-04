using System;
using Microsoft.Azure.Cosmos;

namespace QuickTicket.Storage.CosmosDb
{
    public class DocumentMetadata
    {
        public DocumentMetadata(Type documentType,
            string id,
            PartitionKey partitionKey,
            string etag)
        {
            DocumentType = documentType;
            Id = id;
            PartitionKey = partitionKey;
            Etag = etag;
        }

        public Type DocumentType { get; }
        
        public string Id { get; }

        public PartitionKey PartitionKey { get; }
        
        public string Etag { get; }
    }
}