using System.Collections.Generic;

namespace QuickTicket.Core
{
    public class CountryCode : ValueObject
    {
        public CountryCode(string value)
        {
            Value = value;
        }

        public string Value { get; }

        protected override IEnumerable<object> GetValuesForEqualityCheck()
        {
            yield return Value;
        }
    }
}