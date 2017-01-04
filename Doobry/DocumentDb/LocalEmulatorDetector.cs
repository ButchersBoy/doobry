using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Doobry.Settings;
using Microsoft.Azure.Documents.Client;

namespace Doobry.DocumentDb
{
    public class LocalEmulatorDetector : IDisposable
    {
        private readonly IDisposable _disposable;

        public LocalEmulatorDetector(IImplicitConnectionCache implicitConnectionCache)
        {
            if (implicitConnectionCache == null) throw new ArgumentNullException(nameof(implicitConnectionCache));

            _disposable = Observable
                .Timer(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(3))
                .Do(_ => implicitConnectionCache.Merge("LocalEmulator", SafeGetConnections())).Subscribe();
        }

        private static IEnumerable<Connection> SafeGetConnections()
        {
            try
            {
                return GetConnections();
            }
            catch
            {
                return Enumerable.Empty<ExplicitConnection>();
            }
        }

        private static IEnumerable<Connection> GetConnections()
        {
            var documentClient = CreateDocumentClient();

            return documentClient.CreateDatabaseQuery().AsEnumerable()
                .SelectMany(database =>
                    documentClient.CreateDocumentCollectionQuery(database.SelfLink)
                        .AsEnumerable()
                        .Select(documentCollection =>
                            new Connection(LocalEmulator.Host, LocalEmulator.AuthorisationKey, database.Id,
                                documentCollection.Id))
                ).ToList();
        }

        private static DocumentClient CreateDocumentClient()
        {
            return new DocumentClient(new Uri(LocalEmulator.Host), LocalEmulator.AuthorisationKey);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}