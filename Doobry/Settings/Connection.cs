namespace Doobry.Settings
{
    public class Connection : IConnection
    {
        public Connection(string host, string authorisationKey, string databaseId, string collectionId)
        {            
            Host = host;
            AuthorisationKey = authorisationKey;
            DatabaseId = databaseId;
            CollectionId = collectionId;
        }

        public string Host
        {
            get;            
        }

        public string AuthorisationKey
        {
            get;
        }

        public string DatabaseId
        {
            get; 
        }

        public string CollectionId
        {
            get;
        }

        protected bool Equals(Connection other)
        {
            return string.Equals(Host, other.Host) && string.Equals(AuthorisationKey, other.AuthorisationKey) && string.Equals(DatabaseId, other.DatabaseId) && string.Equals(CollectionId, other.CollectionId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Connection) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Host != null ? Host.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (AuthorisationKey != null ? AuthorisationKey.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (DatabaseId != null ? DatabaseId.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (CollectionId != null ? CollectionId.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}