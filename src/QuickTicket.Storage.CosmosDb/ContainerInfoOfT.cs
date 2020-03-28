using System;
using Microsoft.Azure.Cosmos;

namespace QuickTicket.Storage.CosmosDb
{
    public class ContainerInfo<TDocument> : ContainerInfo
    {
        public ContainerInfo(
            Func<TDocument, PartitionKey> getDocumentPartitionKey,
            Func<TDocument, string> getDocumentId,
            Func<TDocument, string> getDocumentEtag,
            ContainerProperties containerProperties,
            ItemRequestOptions readRequestOptions = null,
            int? throughput = null) : base(
            containerProperties,
            readRequestOptions,
            throughput)
        {
            GetDocumentPartitionKey = getDocumentPartitionKey;
            GetDocumentId = getDocumentId;
            GetDocumentEtag = getDocumentEtag;
        }

        public Func<TDocument, PartitionKey> GetDocumentPartitionKey { get; }

        public Func<TDocument, string> GetDocumentId { get; }
        
        public Func<TDocument, string> GetDocumentEtag { get; }
    }
}