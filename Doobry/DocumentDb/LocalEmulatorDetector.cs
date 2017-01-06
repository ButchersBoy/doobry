using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            if (!SniffEmulator()) return Enumerable.Empty<Connection>();

            var documentClient = CreateDocumentClient();

            var result = documentClient.CreateDatabaseQuery().AsEnumerable()
                .SelectMany(database =>
                {
                    var connections = documentClient.CreateDocumentCollectionQuery(database.SelfLink)
                        .AsEnumerable()
                        .Select(documentCollection =>
                            new Connection(LocalEmulator.Host, LocalEmulator.AuthorisationKey, database.Id,
                                documentCollection.Id)).ToList();

                    if (connections.Count == 0)
                        connections.Add(new Connection(LocalEmulator.Host, LocalEmulator.AuthorisationKey, database.Id,
                            null));

                    return connections;
                }
                ).ToList();

            if (result.Count == 0)
            {
                result.Add(new Connection(LocalEmulator.Host, LocalEmulator.AuthorisationKey, null, null));
            }

            return result;
        }

        private static DocumentClient CreateDocumentClient()
        {
            return new DocumentClient(new Uri(LocalEmulator.Host), LocalEmulator.AuthorisationKey);
        }

        private static bool SniffEmulator()
        {
            return
                System.Diagnostics.Process.GetProcesses()
                    .Any(p => p.ProcessName.StartsWith("DocumentDB.Emulator", StringComparison.InvariantCultureIgnoreCase));
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}