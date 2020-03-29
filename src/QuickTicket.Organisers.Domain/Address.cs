using System.Collections.Generic;
using Ardalis.GuardClauses;
using QuickTicket.Domain;

namespace QuickTicket.Organisers.Domain
{
    public class Address : ValueObject
    {
        public Address(string street,
            string suburb,
            string province,
            string postCode,
            CountryCode countryCode)
        {
            Guard.Against.NullOrEmpty(street, nameof(street));
            Guard.Against.NullOrEmpty(suburb, nameof(suburb));
            Guard.Against.NullOrEmpty(province, nameof(province));
            Guard.Against.NullOrEmpty(postCode, nameof(postCode));
            Street = street;
            Suburb = suburb;
            Province = province;
            PostCode = postCode;
            CountryCode = countryCode;
        }
      
        public string Street { get; }

        public string Suburb { get; }

        public string Province { get; }

        public string PostCode { get; }

        public CountryCode CountryCode { get; }

        protected override IEnumerable<object> GetValuesForEqualityCheck()
        {
            yield return Street;
            yield return Suburb;
            yield return Province;
            yield return PostCode;
            yield return CountryCode;
        }
    }
}