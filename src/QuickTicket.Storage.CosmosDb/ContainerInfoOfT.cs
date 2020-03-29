using System;
using Microsoft.Azure.Cosmos;

namespace QuickTicket.Storage.CosmosDb
{
    public class ContainerInfo<TDocument> : ContainerInfo
    {
        public ContainerInfo(
            Func<TDocument, DocumentMetadata> getDocumentMetadata,
            ContainerProperties containerProperties,
            ItemRequestOptions readRequestOptions = null,
            int? throughput = null) : base(
            containerProperties,
            readRequestOptions,
            throughput)
        {
            GetDocumentMetadata = getDocumentMetadata;
        }

        public Func<TDocument, DocumentMetadata> GetDocumentMetadata { get; }
    }
}