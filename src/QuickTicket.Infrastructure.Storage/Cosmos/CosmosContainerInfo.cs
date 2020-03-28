using Microsoft.Azure.Cosmos;

namespace QuickTicket.Infrastructure.Storage.Cosmos
{
    public abstract class CosmosContainerInfo
    {
        protected CosmosContainerInfo(ContainerProperties containerProperties,
            ItemRequestOptions readRequestOptions = null,
            RequestOptions writeRequestOptions = null,
            int? throughput = null)
        {
            ContainerProperties = containerProperties;
            ReadRequestOptions = readRequestOptions;
            WriteRequestOptions = writeRequestOptions;
            Throughput = throughput;
        }

        public ContainerProperties ContainerProperties { get; }

        public ItemRequestOptions ReadRequestOptions { get; }

        public RequestOptions WriteRequestOptions { get; }

        public int? Throughput { get; }

    }
}