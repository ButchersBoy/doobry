namespace Doobry.Settings
{
    public class Connection
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
    }
}