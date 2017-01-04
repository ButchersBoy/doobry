using System;
using System.Collections.Generic;
using System.Linq;
using DynamicData;

namespace Doobry.Settings
{
    public class ImplicitConnectionCache : IImplicitConnectionCache, IDisposable
    {
        private readonly SourceCache<ImplicitConnection, int> _connectionSoureCache;

        public ImplicitConnectionCache()
        {
            _connectionSoureCache = new SourceCache<ImplicitConnection, int>(cn => cn.GetHashCode());
        }

        public IObservable<IChangeSet<ImplicitConnection, int>> Connect()
        {
            return _connectionSoureCache.Connect();
        }

        public void Merge(string source, IEnumerable<Connection> sourceConnections)
        {
            if (sourceConnections == null) throw new ArgumentNullException(nameof(sourceConnections));
            if (string.IsNullOrEmpty(source)) throw new ArgumentNullException(nameof(source));

            var existingConnections = _connectionSoureCache
                .Items
                .Where(ic => ic.Source.Equals(source))
                .Select(ic => (Connection)ic)
                .ToList();

            var connections = sourceConnections.ToList();
            var removeConnections = existingConnections.Except(connections).Select(cn => (ImplicitConnection)cn);
            var addConnections = connections.Except(existingConnections);

            _connectionSoureCache.Remove(removeConnections);
            _connectionSoureCache.AddOrUpdate(
                addConnections.Select(
                    cn => new ImplicitConnection(source, cn.Host, cn.AuthorisationKey, cn.DatabaseId, cn.CollectionId)));
        }

        public void Dispose()
        {
            _connectionSoureCache.Dispose();
        }
    }
}