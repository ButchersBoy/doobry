using System;
using System.Collections.Generic;
using System.Linq;
using Doobry.Settings;

namespace Doobry.Features.Management
{
    public class GroupedConnection
    {
        private readonly Dictionary<GroupedConnectionKeyLevel, string> _valueIndex;

        public GroupedConnection(GroupedConnectionKey key, IEnumerable<ExplicitConnection> explicitConnections, IEnumerable<ImplicitConnection> implicitConnections)
        {            
            if (explicitConnections == null) throw new ArgumentNullException(nameof(explicitConnections));
            if (implicitConnections == null) throw new ArgumentNullException(nameof(implicitConnections));
            if (key == null) throw new ArgumentNullException(nameof(key));

            ExplicitConnections = explicitConnections;
            ImplicitConnections = implicitConnections;
            Key = key;

            var exemplar = ExplicitConnections.Select(explicitCn => (Connection) explicitCn)
                .Concat(ImplicitConnections.Select(implicitCn => (Connection) implicitCn))
                .FirstOrDefault();

            if (exemplar == null)
                throw new ArgumentException("No connection was provided.");

            _valueIndex = new Dictionary<GroupedConnectionKeyLevel, string>();
            if (key.Level >= (int)GroupedConnectionKeyLevel.Host)
                _valueIndex.Add(GroupedConnectionKeyLevel.Host, exemplar.Host);
            if ((int)key.Level >= (int)GroupedConnectionKeyLevel.AuthorisationKey)
                _valueIndex.Add(GroupedConnectionKeyLevel.AuthorisationKey, exemplar.AuthorisationKey);
            if ((int)key.Level >= (int)GroupedConnectionKeyLevel.DatabaseId)
                _valueIndex.Add(GroupedConnectionKeyLevel.DatabaseId, exemplar.DatabaseId);
            if ((int)key.Level >= (int)GroupedConnectionKeyLevel.CollectionId)
                _valueIndex.Add(GroupedConnectionKeyLevel.CollectionId, exemplar.CollectionId);

        }

        public IEnumerable<ExplicitConnection> ExplicitConnections;

        public IEnumerable<ImplicitConnection> ImplicitConnections;

        public string this[GroupedConnectionKeyLevel keyLevel] => _valueIndex[keyLevel];

        public GroupedConnectionKey Key { get; }
    }
}