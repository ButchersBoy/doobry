using System;

namespace Doobry.Settings
{
    public class Connection
    {
        public Connection(Guid id, string label, string host, string authorisationKey, string databaseId, string collectionId)
        {
            Id = id;
            Label = label;
            Host = host;
            AuthorisationKey = authorisationKey;
            DatabaseId = databaseId;
            CollectionId = collectionId;
        }

        public Guid Id { get; }

        public string Label
        {
            get;
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
    }
}