using System.Collections.Generic;
using QuickTicket.Domain;

namespace QuickTicket.Organisers.Domain
{
    public class WebsiteUrl : ValueObject
    {
        public WebsiteUrl(string value)
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