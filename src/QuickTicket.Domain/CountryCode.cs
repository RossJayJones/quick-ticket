using System.Collections.Generic;
using Ardalis.GuardClauses;

namespace QuickTicket.Domain
{
    public class CountryCode : ValueObject
    {
        public CountryCode(string value)
        {
            Guard.Against.NullOrWhiteSpace(value, nameof(value));
            Value = value;
        }

        public string Value { get; }

        protected override IEnumerable<object> GetValuesForEqualityCheck()
        {
            yield return Value;
        }
    }
}