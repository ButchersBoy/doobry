using System;
using System.Collections.Generic;
using System.Linq;
using Doobry.Settings;

namespace Doobry.Features.Management
{
    /// <summary>
    /// Groups all connections, which effectively amount to the same thing.
    /// </summary>
    public class GroupedConnection : IConnection
    {
        public GroupedConnection(IEnumerable<ExplicitConnection> explicitConnections, IEnumerable<ImplicitConnection> implicitConnections)
        {
            if (explicitConnections == null) throw new ArgumentNullException(nameof(explicitConnections));
            if (implicitConnections == null) throw new ArgumentNullException(nameof(implicitConnections));

            ExplicitConnections = explicitConnections;
            ImplicitConnections = implicitConnections;

            var exemplar = ExplicitConnections.Select(explicitCn => (Connection) explicitCn)
                .Concat(ImplicitConnections.Select(implicitCn => (Connection) implicitCn))
                .FirstOrDefault();

            if (exemplar == null)
                throw new ArgumentException("No connection was provided.");

            Host = exemplar.Host;
            AuthorisationKey = exemplar.AuthorisationKey;
            DatabaseId = exemplar.DatabaseId;
            CollectionId = exemplar.CollectionId;
        }

        public IEnumerable<ExplicitConnection> ExplicitConnections;

        public IEnumerable<ImplicitConnection> ImplicitConnections;

        public string Host { get; }

        public string AuthorisationKey { get; }

        public string DatabaseId { get; }

        public string CollectionId { get; }
    }
}