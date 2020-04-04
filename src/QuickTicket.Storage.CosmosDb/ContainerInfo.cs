using System;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos;

namespace QuickTicket.Storage.CosmosDb
{
    public class ContainerInfo
    {
        private readonly Dictionary<Type, Func<object, DocumentMetadata>> _documentMetadataFactory;
        
        public ContainerInfo(ContainerProperties containerProperties,
            ItemRequestOptions readRequestOptions = null,
            int? throughput = null)
        {
            _documentMetadataFactory = new Dictionary<Type, Func<object, DocumentMetadata>>();
            ContainerProperties = containerProperties;
            ReadRequestOptions = readRequestOptions;
            Throughput = throughput;
        }
        
        public ContainerProperties ContainerProperties { get; }

        public ItemRequestOptions ReadRequestOptions { get; }

        public int? Throughput { get; }

        public ContainerInfo WithDocumentMetadataFactory<TDocument>(Func<TDocument, DocumentMetadata<TDocument>> factory)
        {
            _documentMetadataFactory.Add(typeof(TDocument), obj => factory((TDocument)obj));
            return this;
        }
        
        public DocumentMetadata GetDocumentMetadataForDocument(object document)
        {
            return _documentMetadataFactory[document.GetType()](document);
        }
    }
}