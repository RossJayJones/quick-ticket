using System;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos;

namespace QuickTicket.Storage.CosmosDb
{
    public class DocumentStore : IDocumentStore
    {
        private readonly IReadOnlyDictionary<Type, (Container Container, ContainerInfo ContainerInfo)> _containers;
        
        internal DocumentStore(IReadOnlyDictionary<Type, (Container, ContainerInfo)> containers)
        {
            _containers = containers;
        }
        
        public IDocumentSession<TDocument> CreateSession<TDocument>()
            where TDocument : class
        {
            if (_containers.TryGetValue(typeof(TDocument), out var item))
            {
                return new DocumentSession<TDocument>(
                    container: item.Container, 
                    containerInfo: (ContainerInfo<TDocument>)item.ContainerInfo);
            }

            throw new InvalidOperationException($"A container has not been registered for document type {typeof(TDocument)}");
        }
    }
}