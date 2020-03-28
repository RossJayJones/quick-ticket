using System;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos;

namespace QuickTicket.Infrastructure.Storage.Cosmos
{
    public class CosmosDocumentStore : IDocumentStore
    {
        private readonly IReadOnlyDictionary<Type, (Container Container, CosmosContainerInfo ContainerInfo)> _containers;
        
        internal CosmosDocumentStore(IReadOnlyDictionary<Type, (Container, CosmosContainerInfo)> containers)
        {
            _containers = containers;
        }
        
        public IDocumentSession<TDocument> CreateSession<TDocument>()
        {
            if (_containers.TryGetValue(typeof(TDocument), out var item))
            {
                return new CosmosDocumentSession<TDocument>(item.Container, (CosmosContainerInfo<TDocument>)item.ContainerInfo);
            }

            throw new InvalidOperationException($"A container has not been registered for document type {typeof(TDocument)}");
        }
    }
}