using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickTicket.Core
{
    /// <summary>
    /// Sources: https://enterprisecraftsmanship.com/posts/value-object-better-implementation/
    /// </summary>
    public abstract class ValueObject
    {
        protected abstract IEnumerable<object> GetValuesForEqualityCheck();

        public sealed override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            var valueObject = (ValueObject)obj;

            return GetValuesForEqualityCheck().SequenceEqual(valueObject.GetValuesForEqualityCheck());
        }

        public sealed override int GetHashCode()
        {
            var hashcode = new HashCode();

            foreach (var value in GetValuesForEqualityCheck())
            {
                hashcode.Add(value);
            }

            return hashcode.ToHashCode();
        }
        
        public static bool operator ==(ValueObject a, ValueObject b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(ValueObject a, ValueObject b)
        {
            return !(a == b);
        }
    }
}