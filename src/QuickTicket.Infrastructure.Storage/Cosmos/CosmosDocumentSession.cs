using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace QuickTicket.Infrastructure.Storage.Cosmos
{
    public class CosmosDocumentSession<TDocument> : IDocumentSession<TDocument>
    {
        private readonly Container _container;
        private readonly CosmosContainerInfo<TDocument> _containerInfo;
        private readonly ConcurrentDictionary<string, Task<DocumentState>> _trackedDocuments;
        
        public CosmosDocumentSession(Container container,
            CosmosContainerInfo<TDocument> containerInfo)
        {
            _container = container;
            _containerInfo = containerInfo;
            _trackedDocuments = new ConcurrentDictionary<string, Task<DocumentState>>();
        }

        public async Task<IReadOnlyDictionary<string, TDocument>> LoadMany(IEnumerable<string> documentIds)
        {
            var results = await Task.WhenAll(documentIds.Select(id =>
            {
                var documentState = _trackedDocuments.GetOrAdd(id, async _ =>
                {
                    var response = await _container.ReadItemAsync<TDocument>(
                        id: id,
                        PartitionKey.None,
                        requestOptions: _containerInfo.ReadRequestOptions);
                    return new DocumentState(id, response);
                });
                return documentState;
            }));
            return results.ToDictionary(documentState => documentState.Id, documentState => documentState.Document);
        }
        
        public async Task SaveChanges()
        {
            var partitionKey = await ResolvePartitionKey();
            var batch = _container.CreateTransactionalBatch(partitionKey);

            foreach (var id in _trackedDocuments.Keys.ToList())
            {
                if (_trackedDocuments.TryRemove(id, out var task))
                {
                    var documentState = await task;
                    
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
                        
                        documentState.MarkClean();

                        _trackedDocuments.TryAdd(id, Task.FromResult(documentState));
                    }
                }
            }

            await batch.ExecuteAsync();
        }

        private async Task<PartitionKey> ResolvePartitionKey()
        {
            var distinctPartitionKeys = new HashSet<PartitionKey>();

            foreach (var documentStateTask in _trackedDocuments.Values)
            {
                var documentState = await documentStateTask;
                distinctPartitionKeys.Add(_containerInfo.ResolvePartitionKey(documentState.Document));
            }

            return distinctPartitionKeys.Single();
        }
        
        private class DocumentState
        {
            public DocumentState(string id,
                TDocument document)
            {
                Id = id;
                Document = document;
                IsNew = true;
            }
            
            public DocumentState(string id,
                ItemResponse<TDocument> response)
            {
                Id = id;
                Document = response.Resource;
                ETag = response.ETag;
            }
            
            public string Id { get; }
        
            public TDocument Document { get; }

            public string ETag { get; }

            public bool IsNew { get; private set; }

            public bool IsDeleted { get; private set; }

            public void MarkDeleted()
            {
                IsDeleted = true;
            }

            public void MarkClean()
            {
                IsNew = false;
            }
        }
    }
}