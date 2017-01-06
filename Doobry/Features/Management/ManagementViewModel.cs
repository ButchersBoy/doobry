﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Doobry.DocumentDb;
using Doobry.Infrastructure;
using Doobry.Settings;
using DynamicData;
using DynamicData.Kernel;

namespace Doobry.Features.Management
{
    public class ManagementViewModel : INamed, IDisposable
    {
        private readonly IDisposable _disposable;

        public ManagementViewModel(
            IExplicitConnectionCache explicitConnectionCache, 
            IImplicitConnectionCache implicitConnectionCache,
            IManagementActionsController managementActionsController,
            DispatcherScheduler dispatcherScheduler)
        {
            if (explicitConnectionCache == null) throw new ArgumentNullException(nameof(explicitConnectionCache));
            if (implicitConnectionCache == null) throw new ArgumentNullException(nameof(implicitConnectionCache));
            if (managementActionsController == null)
                throw new ArgumentNullException(nameof(managementActionsController));
            if (dispatcherScheduler == null) throw new ArgumentNullException(nameof(dispatcherScheduler));

            Name = "DB Manager";

            ReadOnlyObservableCollection<HostNode> nodes;
            _disposable = explicitConnectionCache.Connect()
                //user could dupe the connection
                .Group(explicitCn => new GroupedConnectionKey(explicitCn, GroupedConnectionKeyLevel.Host))
                .FullJoin(implicitConnectionCache.Connect().Group(implicitCn => new GroupedConnectionKey(implicitCn, GroupedConnectionKeyLevel.Host)),
                    implicitGroup => implicitGroup.Key,
                    (key, left, right) =>
                        new GroupedConnection(GetOptionalConnections(left), GetOptionalConnections(right), key))
                .Transform(groupedConnection => new HostNode(groupedConnection, managementActionsController))
                .DisposeMany()
                .ObserveOn(dispatcherScheduler)
                .Bind(out nodes)
                .Subscribe();
            Hosts = nodes;
        }

        private static IEnumerable<TConnection> GetOptionalConnections<TConnection, TKey>(Optional<IGroup<TConnection, TKey, GroupedConnectionKey>> left) where TConnection : Connection
        {
            return left.HasValue
                ? left.Value.Cache.Items
                : Enumerable.Empty<TConnection>();
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

        public HostNode(GroupedConnection groupedConnection, IManagementActionsController managementActionsController)
        {
            if (groupedConnection == null) throw new ArgumentNullException(nameof(groupedConnection));
            if (managementActionsController == null)
                throw new ArgumentNullException(nameof(managementActionsController));

            Host = groupedConnection.Host;
            AuthorisationKey = groupedConnection.AuthorisationKey;
            var authKeyHint = groupedConnection.AuthorisationKey ?? "";
            AuthorisationKeyHint =
                (authKeyHint.Length > 0 ? authKeyHint.Substring(0, Math.Min(authKeyHint.Length, 5)) : "")
                + "...";

            CreateDatabaseCommand = new Command(_ => managementActionsController.AddDatabase(this));

            ReadOnlyObservableCollection<DatabaseNode> nodes;
            _disposable = _sourceList.Connect().Bind(out nodes).Subscribe();
            Databases = nodes;

            if (string.IsNullOrEmpty(groupedConnection.DatabaseId)) return;

            _sourceList.Add(string.IsNullOrEmpty(groupedConnection.CollectionId)
                ? new DatabaseNode(this, groupedConnection.DatabaseId)
                : new DatabaseNode(this, groupedConnection.DatabaseId, groupedConnection.CollectionId));
        }

        public ICommand CreateDatabaseCommand { get; }

        public string Host { get; }

        public string AuthorisationKeyHint { get; }

        public string AuthorisationKey { get; }

        public IEnumerable<DatabaseNode> Databases { get; }

        private void CreateDatabase()
        {

        }

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

            CreateCollectionCommand = new Command(_ => CreateCollection());
            DeleteDatabaseCommand = new Command(_ => DeleteDatabase());
        }

        public ICommand DeleteDatabaseCommand { get; }

        public ICommand CreateCollectionCommand { get; }

        public HostNode Owner { get; }

        public string DatabaseId { get; }

        public IEnumerable<CollectionNode> Collections { get; }

        private void DeleteDatabase()
        {

        }

        private void CreateCollection()
        {

        }
    }

    public class CollectionNode
    {
        public CollectionNode(DatabaseNode owner, string collectionId)
        {
            Owner = owner;
            CollectionId = collectionId;

            DeleteCollectionCommand = new Command(_ => DeleteCollection());
        }

        public ICommand DeleteCollectionCommand { get; }

        public DatabaseNode Owner { get; }

        public string CollectionId { get; }

        private void DeleteCollection()
        {

        }
    }
}
