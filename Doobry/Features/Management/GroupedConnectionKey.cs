using System;
using Doobry.Settings;

namespace Doobry.Features.Management
{
    public class GroupedConnectionKey
    {        
        private readonly int _hashCode;

        public GroupedConnectionKey(IConnection connection, GroupedConnectionKeyLevel level)
        {
            Level = level;
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            var hashCode = Concat(GroupedConnectionKeyLevel.Host, connection.Host).GetHashCode();
            if (level == GroupedConnectionKeyLevel.AuthorisationKey)
                hashCode = (hashCode*397) ^
                           Concat(GroupedConnectionKeyLevel.AuthorisationKey, connection.AuthorisationKey).GetHashCode();
            if (level == GroupedConnectionKeyLevel.DatabaseId)
                hashCode = (hashCode * 397) ^
                           Concat(GroupedConnectionKeyLevel.DatabaseId, connection.DatabaseId).GetHashCode();

            _hashCode = hashCode;
        }

        public GroupedConnectionKeyLevel Level { get; }

        protected bool Equals(GroupedConnectionKey other)
        {
            return _hashCode == other._hashCode;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((GroupedConnectionKey) obj);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        private static string Concat(GroupedConnectionKeyLevel level, string value)
        {
            return level + (value ?? "");
        }
    }
}