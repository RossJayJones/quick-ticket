namespace QuickTicket.Organisers.Commands
{
    public interface IAddressData
    {
        public string Street { get; }

        public string Suburb { get; }

        public string Province { get; }

        public string PostCode { get; }

        public string CountryCode { get; }
    }
}