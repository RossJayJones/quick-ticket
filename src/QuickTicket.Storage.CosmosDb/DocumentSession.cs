using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace QuickTicket.Storage.CosmosDb
{
    public class DocumentSession : IDocumentSession
    {
        private readonly Container _container;
        private readonly ContainerInfo _containerInfo;
        private readonly Dictionary<(Type, string), DocumentState> _trackedDocuments;
        
        public DocumentSession(Container container,
            ContainerInfo containerInfo)
        {
            _container = container;
            _containerInfo = containerInfo;
            _trackedDocuments = new Dictionary<(Type, string), DocumentState>();
        }

        public void Add<TDocument>(TDocument document)
        {
            var documentMetadata = _containerInfo.GetDocumentMetadataForDocument(document);
            var documentState = new DocumentState<TDocument>(documentMetadata,
                document);

            if (!_trackedDocuments.TryAdd((typeof(TDocument), documentState.Metadata.Id), documentState))
            {
                throw new InvalidOperationException($"A document with id {documentState.Metadata.Id} has already been added to the session");
            }
        }

        public void Update<TDocument>(TDocument document)
        {
            var documentMetadata = _containerInfo.GetDocumentMetadataForDocument(document);
            if (_trackedDocuments.TryGetValue((typeof(TDocument), documentMetadata.Id),
                out var documentState))
            {
                ((DocumentState<TDocument>)documentState).Document = document;
            }
            else
            {
                throw new DocumentSessionException($"Document with id {documentMetadata.Id} is not tracked by the session");
            }
        }

        public void Remove<TDocument>(TDocument document)
        {
            var documentMetadata = _containerInfo.GetDocumentMetadataForDocument(document);
            if (_trackedDocuments.TryGetValue((typeof(TDocument), documentMetadata.Id),
                out var documentState))
            {
                documentState.MarkDeleted();
            }
            else
            {
                throw new DocumentSessionException($"Document with id {documentMetadata.Id} is not tracked by the session");
            }
        }

        public async Task<IReadOnlyDictionary<string, TDocument>> LoadMany<TDocument>(IEnumerable<string> documentIds)
        {
            var results = new Dictionary<string, DocumentState>();
            var documentsToLoad = new List<string>();
            
            foreach (var documentId in documentIds.Distinct())
            {
                if (_trackedDocuments.TryGetValue((typeof(TDocument), documentId), out var documentState)
                    && !documentState.IsDeleted)
                {
                    results.Add(documentState.Metadata.Id, (DocumentState<TDocument>)documentState);
                }
                else
                {
                    results.Add(documentId, DocumentState.Empty);
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
                        var documentMetadata = _containerInfo.GetDocumentMetadataForDocument(document);
                        var documentState = new DocumentState<TDocument>(documentMetadata, document);
                        _trackedDocuments.TryAdd((typeof(TDocument), documentMetadata.Id), documentState);
                        results[documentMetadata.Id] = documentState;
                    }
                }
            }

            return results.ToDictionary(p => p.Key, p => p.Value == DocumentState.Empty
                ? default
                : (TDocument)p.Value.Document);
        }
        
        public async Task SaveChanges()
        {
            if (!_trackedDocuments.Any())
            {
                return;
            }
            
            var partitionKey = ResolvePartitionKey();
            var batch = _container.CreateTransactionalBatch(partitionKey);
            var documentTypesInBatch = new List<Type>();

            foreach (var id in _trackedDocuments.Keys.ToList())
            {
                if (_trackedDocuments.Remove(id, out var documentState))
                {
                    documentTypesInBatch.Add(documentState.Metadata.DocumentType);
                    
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

            var response = await batch.ExecuteAsync();
            
            UpdateEtagFromResponseUtils.UpdateEtagFromResponse(this, documentTypesInBatch, response);
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

        private void UpdateEtagFromResponse<TDocument>(TransactionalBatchResponse response, int index)
        {
            var result = response.GetOperationResultAtIndex<TDocument>(index);

            if (!result.IsSuccessStatusCode)
            {
                throw new DocumentSessionException($"Received response status {result.StatusCode} from Cosmos Db");
            }

            // Update the Etag to latest value
            var documentMetadata = _containerInfo.GetDocumentMetadataForDocument(result.Resource);
            if (_trackedDocuments.TryGetValue((documentMetadata.DocumentType, documentMetadata.Id),
                out var documentState))
            {
                documentState.Document = result.Resource;
                documentState.Metadata = documentMetadata;
            }
        }

        private class DocumentState
        {
            protected DocumentState(DocumentMetadata metadata)
            {
                Metadata = metadata;
            }
            
            public DocumentMetadata Metadata { get; set; }
        
            public bool IsDeleted { get; private set; }

            public void MarkDeleted()
            {
                IsDeleted = true;
            }

            public object Document { get; set; }
            
            public static readonly DocumentState Empty = new DocumentState(new DocumentMetadata(typeof(object), null, PartitionKey.Null, null));
        }

        private class DocumentState<TDocument> : DocumentState
        {
            public DocumentState(DocumentMetadata metadata,
                TDocument document) : base(metadata)
            {
                Document = document;
            }
        }
        
        /// <summary>
        /// This is a workaround for the lack of a non generic option to call GetOperationResultAtIndex.
        /// </summary>
        private static class UpdateEtagFromResponseUtils
        {
            private static readonly MethodInfo Method;
            private static readonly ConcurrentDictionary<Type, MethodInfo> GenericMethods;
            
            static UpdateEtagFromResponseUtils()
            {
                Method = typeof(DocumentSession).GetMethod(nameof(DocumentSession.UpdateEtagFromResponse), BindingFlags.Instance | BindingFlags.NonPublic);
                GenericMethods = new ConcurrentDictionary<Type, MethodInfo>();
            }

            public static void UpdateEtagFromResponse(DocumentSession session,
                IList<Type> types,
                TransactionalBatchResponse response)
            {
                try
                {
                    for (int i = 0; i < response.Count; i++)
                    {
                        var method = GenericMethods.GetOrAdd(types[i], item => Method.MakeGenericMethod(item));
                        method.Invoke(session, new object[] {response, i});
                    }
                }
                catch (TargetInvocationException e) when(e.InnerException != null) /* Unpacks the inner exception */
                {
                    throw e.InnerException;
                }
            }
        }
    }
}