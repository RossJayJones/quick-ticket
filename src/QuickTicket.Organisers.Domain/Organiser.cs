using Ardalis.GuardClauses;
using QuickTicket.Core;

namespace QuickTicket.Organisers.Domain
{
    public class Organiser
    {
        public static Organiser Create(string name,
            string description,
            ContactNumber contactNumber,
            EmailAddress emailAddress,
            WebsiteUrl websiteUrl,
            Address physicalAddress,
            Address postalAddress)
        {
            Guard.Against.NullOrEmpty(name, nameof(name));
            Guard.Against.NullOrEmpty(description, nameof(description));
            Guard.Against.Null(contactNumber, nameof(contactNumber));
            Guard.Against.Null(emailAddress, nameof(emailAddress));
            Guard.Against.Null(websiteUrl, nameof(websiteUrl));
            Guard.Against.Null(physicalAddress, nameof(physicalAddress));
            Guard.Against.Null(postalAddress, nameof(postalAddress));
            var organiser = new Organiser(
                OrganiserId.New(),
                name,
                description,
                contactNumber,
                emailAddress,
                websiteUrl,
                physicalAddress,
                postalAddress);
            return organiser;
        }

        private Organiser(OrganiserId organiserId,
            string name,
            string description,
            ContactNumber contactNumber,
            EmailAddress emailAddress,
            WebsiteUrl websiteUrl,
            Address physicalAddress,
            Address postalAddress)
        {
            OrganiserId = organiserId;
            Name = name;
            Description = description;
            ContactNumber = contactNumber;
            EmailAddress = emailAddress;
            WebsiteUrl = websiteUrl;
            PhysicalAddress = physicalAddress;
            PostalAddress = postalAddress;
        }
        
        public OrganiserId OrganiserId { get; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public ContactNumber ContactNumber { get; private set; }

        public EmailAddress EmailAddress { get; private set; }

        public WebsiteUrl WebsiteUrl { get; private set; }

        public Address PhysicalAddress { get; private set; }

        public Address PostalAddress { get; private set; }

        public void SetName(string name)
        {
            Guard.Against.NullOrEmpty(name, nameof(name));
            Name = name;
        }

        public void SetDescription(string description)
        {
            Guard.Against.NullOrEmpty(description, nameof(description));
            Description = description;
        }
        
        public void SetContactDetails(ContactNumber contactNumber,
            EmailAddress emailAddress,
            WebsiteUrl websiteUrl)
        {
            Guard.Against.Null(contactNumber, nameof(contactNumber));
            Guard.Against.Null(EmailAddress, nameof(EmailAddress));
            Guard.Against.Null(WebsiteUrl, nameof(WebsiteUrl));
            ContactNumber = contactNumber;
            EmailAddress = emailAddress;
            WebsiteUrl = websiteUrl;
        }

        public void SetPhysicalAddress(Address physicalAddress)
        {
            Guard.Against.Null(physicalAddress, nameof(physicalAddress));
            PhysicalAddress = physicalAddress;
        }

        public void SetPostalAddress(Address postalAddress)
        {
            Guard.Against.Null(postalAddress, nameof(postalAddress));
            PostalAddress = postalAddress;
        }
    }
}