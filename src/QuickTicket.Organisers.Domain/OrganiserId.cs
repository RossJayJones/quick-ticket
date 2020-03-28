using System;
using System.Collections.Generic;
using QuickTicket.Domain;

namespace QuickTicket.Organisers.Domain
{
    public class OrganiserId : ValueObject
    {
        public static OrganiserId New()
        {
            return new OrganiserId(Guid.NewGuid());
        }
        
        private OrganiserId(Guid value)
        {
            Value = value;
        }
        
        public Guid Value { get; }

        protected override IEnumerable<object> GetValuesForEqualityCheck()
        {
            yield return Value;
        }
    }
}