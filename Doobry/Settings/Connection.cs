namespace Doobry.Settings
{
    public class Connection
    {
        public Connection(string label, string host, string authorisationKey, string databaseId, string collectionId)
        {
            Label = label;
            AuthorisationKey = authorisationKey;
            DatabaseId = databaseId;
            CollectionId = collectionId;
        }

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