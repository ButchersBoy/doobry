using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Kernel;

namespace Doobry.Settings
{
    public interface IConnectionCache : IEnumerable<Connection>
    {
        Optional<Connection> Get(Guid id);

        IObservable<IChangeSet<Connection, Guid>> Connect();
        void AddOrUpdate(Connection connection);
        void Delete(Guid id);
    }

    public class ConnectionCache : IConnectionCache, IDisposable
    {
        private readonly SourceCache<Connection, Guid> _connectionSoureCache;

        public ConnectionCache() : this(Enumerable.Empty<Connection>())
        { }

        public ConnectionCache(IEnumerable<Connection> connections)
        {
            _connectionSoureCache = new SourceCache<Connection, Guid>(cn => cn.Id);
            _connectionSoureCache.AddOrUpdate(connections ?? Enumerable.Empty<Connection>());            
        }

        public Optional<Connection> Get(Guid id)
        {
            return _connectionSoureCache.Lookup(id);
        }

        public IObservable<IChangeSet<Connection, Guid>> Connect()
        {
            return _connectionSoureCache.Connect();
        }

        public void AddOrUpdate(Connection connection)
        {            
            _connectionSoureCache.AddOrUpdate(connection);
        }

        public void Delete(Guid id)
        {
            _connectionSoureCache.Remove(id);
        }

        public void Dispose()
        {
            _connectionSoureCache.Dispose();
        }

        public IEnumerator<Connection> GetEnumerator()
        {
            return _connectionSoureCache.Items.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _connectionSoureCache.Items.ToList().GetEnumerator();
        }
    }

}
