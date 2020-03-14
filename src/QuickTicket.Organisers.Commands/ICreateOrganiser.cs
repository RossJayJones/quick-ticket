namespace QuickTicket.Organisers.Commands
{
    public interface ICreateOrganiser
    {
        public string Name { get; }

        public string Description { get; }

        public string EmailAddress { get; }

        public string ContactNumber { get; }

        public string WebsiteUrl { get; }
    }
}