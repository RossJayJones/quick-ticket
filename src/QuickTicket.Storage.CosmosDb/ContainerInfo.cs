using Microsoft.Azure.Cosmos;

namespace QuickTicket.Storage.CosmosDb
{
    public abstract class ContainerInfo
    {
        protected ContainerInfo(ContainerProperties containerProperties,
            ItemRequestOptions readRequestOptions = null,
            int? throughput = null)
        {
            ContainerProperties = containerProperties;
            ReadRequestOptions = readRequestOptions;
            Throughput = throughput;
        }

        public ContainerProperties ContainerProperties { get; }

        public ItemRequestOptions ReadRequestOptions { get; }

        public int? Throughput { get; }
    }
}