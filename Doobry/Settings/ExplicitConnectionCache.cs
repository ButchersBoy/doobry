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
    public class ExplicitConnectionCache : IExplicitConnectionCache, IDisposable
    {
        private readonly SourceCache<ExplicitConnection, Guid> _connectionSoureCache;

        public ExplicitConnectionCache() : this(Enumerable.Empty<ExplicitConnection>())
        { }

        public ExplicitConnectionCache(IEnumerable<ExplicitConnection> connections)
        {
            _connectionSoureCache = new SourceCache<ExplicitConnection, Guid>(cn => cn.Id);
            _connectionSoureCache.AddOrUpdate(connections ?? Enumerable.Empty<ExplicitConnection>());            
        }

        public Optional<ExplicitConnection> Get(Guid id)
        {
            return _connectionSoureCache.Lookup(id);
        }

        public IObservable<IChangeSet<ExplicitConnection, Guid>> Connect()
        {
            return _connectionSoureCache.Connect();
        }

        public void AddOrUpdate(ExplicitConnection explicitConnection)
        {            
            _connectionSoureCache.AddOrUpdate(explicitConnection);
        }

        public void Delete(Guid id)
        {
            _connectionSoureCache.Remove(id);
        }

        public void Dispose()
        {
            _connectionSoureCache.Dispose();
        }

        public IEnumerator<ExplicitConnection> GetEnumerator()
        {
            return _connectionSoureCache.Items.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _connectionSoureCache.Items.ToList().GetEnumerator();
        }
    }
}
