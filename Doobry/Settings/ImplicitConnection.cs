using System;

namespace Doobry.Settings
{
    public class ImplicitConnection : Connection
    {
        public ImplicitConnection(string source, string host, string authorisationKey, string databaseId, string collectionId) 
            : base(host, authorisationKey, databaseId, collectionId)
        {
            if (string.IsNullOrEmpty(source)) throw new ArgumentNullException(nameof(source));

            Source = source;
        }

        public string Source { get; }
    }
}