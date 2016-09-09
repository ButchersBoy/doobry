using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doobry.Settings
{
    public class ConnectionCache : IEnumerable<Connection>
    {
        private readonly IDictionary<Guid, Connection> _connectionIndex = new Dictionary<Guid, Connection>();

        public ConnectionCache() : this(Enumerable.Empty<Connection>())
        {
            
        }

        public ConnectionCache(IEnumerable<Connection> connections)
        {
            Reset(connections);
        }

        public void Reset(IEnumerable<Connection> connections)
        {
            if (connections == null) throw new ArgumentNullException(nameof(connections));

            _connectionIndex.Clear();
            foreach (var connection in connections)
            {
                _connectionIndex.Add(connection.Id, connection);
            }
        }

        public Connection Get(Guid id)
        {
            return _connectionIndex[id];
        }

        public bool TryGet(Guid id, out Connection connection)
        {
            return _connectionIndex.TryGetValue(id, out connection);
        }

        public IEnumerator<Connection> GetEnumerator()
        {
            return _connectionIndex.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public static class Extensions
    {
        public static Connection ToConnection(this ConnectionEditorViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            return new Connection(viewModel.Id ?? Guid.NewGuid(), 
                viewModel.Label,
                viewModel.Host,
                viewModel.AuthorisationKey,
                viewModel.DatabaseId,
                viewModel.CollectionId);
        }

        public static ConnectionEditorViewModel ToViewModel(this Connection connection)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            return new ConnectionEditorViewModel(connection.Id)
            {                
                AuthorisationKey = connection.AuthorisationKey,
                CollectionId = connection.CollectionId,
                DatabaseId = connection.DatabaseId,
                Host = connection.Host,
                Label = connection.Label
            };
        }
    }
}
