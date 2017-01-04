using System.ComponentModel;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using DynamicData.Kernel;

namespace Doobry.DocumentDb
{
    public static class LocalEmulator
    {
        //TODO allow port to be overriden
        public static string Host = "https://localhost:8081";

        public static string AuthorisationKey =
            "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
    }

    /*

    public class LocalEmulatorDetector
    {
        private IEnumerable<ExplicitConnection> _latestConnections = null;

        public LocalEmulatorDetector()
        {
            
            var refCount = Observable
                .Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
                .Select(_ =>
                {
                    CreateDocumentClient().GetDatabaseAccountAsync().Result.DatabasesLink
                })
                .Publish().RefCount();
            

            
        }

        /// <summary>
        /// Watches for an active local emulator collections.
        /// </summary>
        /// <returns></returns>
        public static IObservable<IEnumerable<ExplicitConnection>> Watch()
        {
            return null;            
        }

        private IEnumerable<ExplicitConnection> SafeGetAllConnections()
        {
            try
            {
                return GetAllConnections();
            }
            catch
            {
                return Enumerable.Empty<ExplicitConnection>();
            }
        }

        private IEnumerable<ExplicitConnection> GetAllConnections()
        {
            var documentClient = CreateDocumentClient();

            documentClient.CreateDatabaseQuery().AsEnumerable()
                .SelectMany(database =>
                {                    
                    documentClient.CreateDocumentCollectionQuery(database.SelfLink).Select(documentCollection =>
                    documentCollection.
                    new ExplicitConnection(documentCollection.))
                })


        }

        private static DocumentClient CreateDocumentClient()
        {
            return new DocumentClient(new Uri(LocalEmulator.Host), LocalEmulator.AuthorisationKey);
        }
    }
*/
}
