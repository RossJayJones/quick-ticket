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
        private readonly ConcurrentDictionary<string, DocumentState> _trackedDocuments;
        
        public DocumentSession(Container container,
            ContainerInfo<TDocument> containerInfo)
        {
            _container = container;
            _containerInfo = containerInfo;
            _trackedDocuments = new ConcurrentDictionary<string, DocumentState>();
        }

        public void Add(TDocument document)
        {
            var documentState = new DocumentState(_containerInfo.GetDocumentId(document),
                document);

            if (!_trackedDocuments.TryAdd(documentState.Id, documentState))
            {
                throw new InvalidOperationException($"A document with id {documentState.Id} has already been added to the session");
            }
        }

        public async Task<IReadOnlyDictionary<string, TDocument>> LoadMany(IEnumerable<string> documentIds)
        {
            var results = new Dictionary<string, DocumentState>();
            var documentsToLoad = new List<string>();
            
            foreach (var documentId in documentIds.Distinct())
            {
                if (_trackedDocuments.TryGetValue(documentId, out var documentState))
                {
                    results.Add(documentState.Id, documentState);
                }
                else
                {
                    results.Add(documentId, null);
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
                        var documentId = _containerInfo.GetDocumentId(document);
                        var documentState = new DocumentState(documentId, document,
                            _containerInfo.GetDocumentEtag(document));
                        _trackedDocuments.TryAdd(documentId, documentState);
                        results[documentId] = documentState;
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
                if (_trackedDocuments.TryRemove(id, out var documentState))
                {
                    if (documentState.IsDeleted)
                    {
                        batch.DeleteItem(documentState.Id, new TransactionalBatchItemRequestOptions
                        {
                            IfMatchEtag = documentState.ETag
                        });
                    }
                    else
                    {
                        batch.UpsertItem(documentState.Document, new TransactionalBatchItemRequestOptions
                        {
                            IfMatchEtag = documentState.ETag
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
                if (_trackedDocuments.TryGetValue(_containerInfo.GetDocumentId(response.Resource),
                    out var documentState))
                {
                    documentState.ETag = response.ETag;
                }
            }
        }

        private PartitionKey ResolvePartitionKey()
        {
            var distinctPartitionKeys = new HashSet<PartitionKey>();

            foreach (var documentState in _trackedDocuments.Values)
            {
                distinctPartitionKeys.Add(_containerInfo.GetDocumentPartitionKey(documentState.Document));
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
            public DocumentState(string id,
                TDocument document)
            {
                Id = id;
                Document = document;
            }
            
            public DocumentState(string id,
                TDocument document,
                string eTag)
            {
                Id = id;
                Document = document;
                ETag = eTag;
            }
            
            public string Id { get; }
        
            public TDocument Document { get; }

            public string ETag { get; set; }

            public bool IsDeleted { get; private set; }

            public void MarkDeleted()
            {
                IsDeleted = true;
            }
        }
    }
}