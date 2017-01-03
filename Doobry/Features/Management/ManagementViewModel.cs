using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Doobry.Settings;
using DynamicData;

namespace Doobry.Features.Management
{
    public class ManagementViewModel : INamed, IDisposable
    {
        private readonly IDisposable _disposable;

        public ManagementViewModel(IConnectionCache connectionCache)
        {
            if (connectionCache == null) throw new ArgumentNullException(nameof(connectionCache));

            Name = "DB Manager";

            ReadOnlyObservableCollection<HostNode> nodes;
            _disposable = connectionCache.Connect().Transform(cn => new HostNode(cn)).DisposeMany().Bind(out nodes)
                .Subscribe();
            Hosts = nodes;
        }

        public string Name { get; }

        public IEnumerable<HostNode> Hosts { get; }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }

    public class HostNode : IDisposable
    {
        private readonly SourceList<DatabaseNode> _sourceList = new SourceList<DatabaseNode>();
        private readonly IDisposable _disposable;

        public HostNode(Connection cn)
        {
            Host = cn.Host;
            AuthorisationKey = cn.AuthorisationKey;
            var authKeyHint = cn.AuthorisationKey ?? "";
            AuthorisationKeyHint =
                (authKeyHint.Length > 0 ? authKeyHint.Substring(0, Math.Min(authKeyHint.Length, 5)) : "")
                + "...";

            ReadOnlyObservableCollection<DatabaseNode> nodes;
            _disposable = _sourceList.Connect().Bind(out nodes).Subscribe();
            Databases = nodes;

            if (string.IsNullOrEmpty(cn.DatabaseId)) return;

            _sourceList.Add(string.IsNullOrEmpty(cn.CollectionId)
                ? new DatabaseNode(this, cn.DatabaseId)
                : new DatabaseNode(this, cn.DatabaseId, cn.CollectionId));
        }

        public string Host { get; }

        public string AuthorisationKeyHint { get; }

        public string AuthorisationKey { get; }

        public IEnumerable<DatabaseNode> Databases { get; }
        public void Dispose()
        {
            _disposable.Dispose();
        }
    }

    public class DatabaseNode
    {
        public DatabaseNode(HostNode owner, string databaseId, params string[] collectionIds)
        {
            Owner = owner;
            DatabaseId = databaseId;
            Collections = collectionIds.Select(c => new CollectionNode(this, c));
        }

        public HostNode Owner { get; }

        public string DatabaseId { get; }

        public IEnumerable<CollectionNode> Collections { get; }
    }

    public class CollectionNode
    {
        public CollectionNode(DatabaseNode owner, string collectionId)
        {
            Owner = owner;
            CollectionId = collectionId;
        }

        public DatabaseNode Owner { get; }

        public string CollectionId { get; }
    }
}
