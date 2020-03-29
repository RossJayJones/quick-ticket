namespace QuickTicket.Organisers.Domain.Infrastructure.Documents
{
    public class AddressDocument
    {
        public string Street { get; set; }

        public string Suburb { get; set; }

        public string Province { get; set; }

        public string PostCode { get; set; }

        public string CountryCode { get; set; }
    }
}