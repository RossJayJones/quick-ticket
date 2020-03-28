using System;
using System.Linq.Expressions;
using Microsoft.Azure.Cosmos;

namespace QuickTicket.Infrastructure.Storage.Cosmos
{
    public class CosmosContainerInfo<TDocument> : CosmosContainerInfo
    {
        public CosmosContainerInfo(
            Expression<Func<TDocument, bool>> findByIdExpression,
            ContainerProperties containerProperties,
            ItemRequestOptions readRequestOptions = null,
            RequestOptions writeRequestOptions = null,
            int? throughput = null) : base(
            containerProperties,
            readRequestOptions,
            writeRequestOptions,
            throughput)
        {
            FindByIdExpression = findByIdExpression;
        }

        public Expression<Func<TDocument, bool>> FindByIdExpression { get; }

        public PartitionKey ResolvePartitionKey(TDocument document)
        {
            throw new NotImplementedException();
        }

        public string ResolveId(TDocument document)
        {
            throw new NotImplementedException();
        }
    }
}