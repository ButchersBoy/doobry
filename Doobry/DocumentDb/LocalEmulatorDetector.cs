using System;
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
    public class LocalEmulatorDetector : IObservable<LocalEmulatorDetectorUnit>
    {
        private readonly IObservable<LocalEmulatorDetectorUnit> _observable;

        public LocalEmulatorDetector(IImplicitConnectionCache implicitConnectionCache)
        {
            if (implicitConnectionCache == null) throw new ArgumentNullException(nameof(implicitConnectionCache));

            _observable = Observable
                .Timer(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(3))
                .Select(_ => SafeGetConnections())
                .Publish()
                .RefCount();
        }

        private static LocalEmulatorDetectorUnit SafeGetConnections()
        {
            try
            {
                return GetConnections();
            }
            catch
            {
                return new LocalEmulatorDetectorUnit(false, Enumerable.Empty<ExplicitConnection>());
            }
        }

        private static LocalEmulatorDetectorUnit GetConnections()
        {
            if (!SniffEmulator()) return new LocalEmulatorDetectorUnit(false, Enumerable.Empty<ExplicitConnection>());

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

            return new LocalEmulatorDetectorUnit(true, result);
        }

        private static DocumentClient CreateDocumentClient()
        {
            var connectionPolicy = new ConnectionPolicy
            {
                RequestTimeout = TimeSpan.FromSeconds(1)
            };
            return new DocumentClient(new Uri(LocalEmulator.Host), LocalEmulator.AuthorisationKey, connectionPolicy);
        }

        private static bool SniffEmulator()
        {
            return
                System.Diagnostics.Process.GetProcesses()
                    .Any(p => p.ProcessName.StartsWith("DocumentDB.Emulator", StringComparison.InvariantCultureIgnoreCase));
        }

        public IDisposable Subscribe(IObserver<LocalEmulatorDetectorUnit> observer)
        {
            return _observable.Subscribe(observer);
        }
    }
}