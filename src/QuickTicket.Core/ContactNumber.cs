using System.Collections.Generic;

namespace QuickTicket.Core
{
    public class ContactNumber : ValueObject
    {
        public ContactNumber(string value)
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