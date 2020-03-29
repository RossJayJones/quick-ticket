using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace QuickTicket.Storage.CosmosDb
{
    public class DocumentSession<TDocument> : IDocumentSession<TDocument>
        where TDocument : class
    {
        private readonly Container _container;
        private readonly ContainerInfo<TDocument> _containerInfo;
        private readonly Dictionary<string, DocumentState> _trackedDocuments;
        
        public DocumentSession(Container container,
            ContainerInfo<TDocument> containerInfo)
        {
            _container = container;
            _containerInfo = containerInfo;
            _trackedDocuments = new Dictionary<string, DocumentState>();
        }

        public void Add(TDocument document)
        {
            var documentMetadata = _containerInfo.GetDocumentMetadata(document);
            var documentState = new DocumentState(documentMetadata,
                document);

            if (!_trackedDocuments.TryAdd(documentState.Metadata.Id, documentState))
            {
                throw new InvalidOperationException($"A document with id {documentState.Metadata.Id} has already been added to the session");
            }
        }

        public void Update(TDocument document)
        {
            var documentMetadata = _containerInfo.GetDocumentMetadata(document);
            if (_trackedDocuments.TryGetValue(documentMetadata.Id,
                out var documentState))
            {
                documentState.Document = document;
            }
            else
            {
                throw new DocumentSessionException($"Document with id {documentMetadata.Id} is not tracked by the session");
            }
        }

        public void Remove(TDocument document)
        {
            var documentMetadata = _containerInfo.GetDocumentMetadata(document);
            if (_trackedDocuments.TryGetValue(documentMetadata.Id,
                out var documentState))
            {
                documentState.MarkDeleted();
            }
            else
            {
                throw new DocumentSessionException($"Document with id {documentMetadata.Id} is not tracked by the session");
            }
        }

        public async Task<IReadOnlyDictionary<string, TDocument>> LoadMany(IEnumerable<string> documentIds)
        {
            var results = new Dictionary<string, DocumentState>();
            var documentsToLoad = new List<string>();
            
            foreach (var documentId in documentIds.Distinct())
            {
                if (_trackedDocuments.TryGetValue(documentId, out var documentState)
                    && !documentState.IsDeleted)
                {
                    results.Add(documentState.Metadata.Id, documentState);
                }
                else
                {
                    results.Add(documentId, null);
                }

                if (documentState == null)
                {
                    documentsToLoad.Add(documentId);
                }
            }

            if (documentsToLoad.Any())
            {
                var queryDefinition = CreateQueryDefinition(documentsToLoad);
                var queryIterator = _container.GetItemQueryIterator<TDocument>(queryDefinition);

                while (queryIterator.HasMoreResults)
                {
                    foreach (var document in await queryIterator.ReadNextAsync())
                    {
                        var documentMetadata = _containerInfo.GetDocumentMetadata(document);
                        var documentState = new DocumentState(documentMetadata, document);
                        _trackedDocuments.TryAdd(documentMetadata.Id, documentState);
                        results[documentMetadata.Id] = documentState;
                    }
                }
            }

            return results.ToDictionary(p => p.Key, p => p.Value?.Document);
        }
        
        public async Task SaveChanges()
        {
            if (!_trackedDocuments.Any())
            {
                return;
            }
            
            var partitionKey = ResolvePartitionKey();
            var batch = _container.CreateTransactionalBatch(partitionKey);

            foreach (var id in _trackedDocuments.Keys.ToList())
            {
                if (_trackedDocuments.Remove(id, out var documentState))
                {
                    if (documentState.IsDeleted)
                    {
                        batch.DeleteItem(documentState.Metadata.Id, new TransactionalBatchItemRequestOptions
                        {
                            IfMatchEtag = documentState.Metadata.Etag
                        });
                    }
                    else
                    {
                        batch.UpsertItem(documentState.Document, new TransactionalBatchItemRequestOptions
                        {
                            IfMatchEtag = documentState.Metadata.Etag
                        });
                        
                        _trackedDocuments.TryAdd(id, documentState);
                    }
                }
            }

            var result = await batch.ExecuteAsync();

            for (int i = 0; i < result.Count; i++)
            {
                var response = result.GetOperationResultAtIndex<TDocument>(i);

                if (!response.IsSuccessStatusCode)
                {
                    throw new DocumentSessionException($"Received response status {response.StatusCode} from Cosmos Db");
                }

                // Update the Etag to latest value
                var documentMetadata = _containerInfo.GetDocumentMetadata(response.Resource);
                if (_trackedDocuments.TryGetValue(documentMetadata.Id,
                    out var documentState))
                {
                    documentState.Document = response.Resource;
                    documentState.Metadata = documentMetadata;
                }
            }
        }

        private PartitionKey ResolvePartitionKey()
        {
            var distinctPartitionKeys = new HashSet<PartitionKey>();

            foreach (var documentState in _trackedDocuments.Values)
            {
                distinctPartitionKeys.Add(documentState.Metadata.PartitionKey);
            }

            return distinctPartitionKeys.Single();
        }
        
        private QueryDefinition CreateQueryDefinition(IList<string> documentsToLoad)
        {
            var parameters = documentsToLoad.Select(p => $"@p{documentsToLoad.IndexOf(p)}");
            var queryText = $"SELECT * FROM c WHERE c.id IN ({string.Join(",", parameters)})";
            var queryDefinition = new QueryDefinition(queryText);
            foreach (var documentId in documentsToLoad)
            {
                queryDefinition = queryDefinition.WithParameter($"@p{documentsToLoad.IndexOf(documentId)}", documentId);
            }
            return queryDefinition;
        }

        private class DocumentState
        {
            public DocumentState(DocumentMetadata metadata,
                TDocument document)
            {
                Metadata = metadata;
                Document = document;
            }
            
            public DocumentMetadata Metadata { get; set; }
        
            public TDocument Document { get; set; }

            public bool IsDeleted { get; private set; }

            public void MarkDeleted()
            {
                IsDeleted = true;
            }
        }
    }
}