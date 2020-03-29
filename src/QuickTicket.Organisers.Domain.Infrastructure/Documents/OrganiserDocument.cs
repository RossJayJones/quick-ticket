using Newtonsoft.Json;

namespace QuickTicket.Organisers.Domain.Infrastructure.Documents
{
    public class OrganiserDocument
    {
        [JsonProperty("id")]
        public string OrganiserId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ContactNumber { get; set; }

        public string EmailAddress { get; set; }

        public string WebsiteUrl { get; set; }

        public AddressDocument PhysicalAddress { get; set; }

        public AddressDocument PostalAddress { get; set; }
        
        [JsonProperty("_etag")]
        public string Etag { get; set; }

        public string PartitionKey { get; set; }
    }
}