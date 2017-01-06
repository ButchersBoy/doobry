using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using System.Windows.Input;
using Doobry.Infrastructure;
using Doobry.Settings;

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

            _disposable = explicitConnectionCache.BuildChildNodes(
                implicitConnectionCache,
                null,
                GroupedConnectionKeyLevel.AuthorisationKey, 
                dispatcherScheduler,
                groupedConnection =>
                    new HostNode(groupedConnection, managementActionsController, explicitConnectionCache,
                        implicitConnectionCache, dispatcherScheduler), out nodes);

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
        private readonly IDisposable _disposable;

        public HostNode(
            GroupedConnection groupedConnection, 
            IManagementActionsController managementActionsController,
            IExplicitConnectionCache explicitConnectionCache,
            IImplicitConnectionCache implicitConnectionCache,
            DispatcherScheduler dispatcherScheduler
            )
        {
            if (groupedConnection == null) throw new ArgumentNullException(nameof(groupedConnection));
            if (managementActionsController == null)
                throw new ArgumentNullException(nameof(managementActionsController));
            if (groupedConnection.Key.Level != GroupedConnectionKeyLevel.AuthorisationKey)
                throw new ArgumentException($"Expected key level of {GroupedConnectionKeyLevel.AuthorisationKey}.", nameof(groupedConnection));

            Host = groupedConnection[GroupedConnectionKeyLevel.Host];
            AuthorisationKey = groupedConnection[GroupedConnectionKeyLevel.AuthorisationKey];
            var authKeyHint = AuthorisationKey ?? "";
            AuthorisationKeyHint =
                (authKeyHint.Length > 0 ? authKeyHint.Substring(0, Math.Min(authKeyHint.Length, 5)) : "")
                + "...";

            CreateDatabaseCommand = new Command(_ => managementActionsController.AddDatabase(this));

            ReadOnlyObservableCollection<DatabaseNode> nodes;
            _disposable = explicitConnectionCache.BuildChildNodes(
                implicitConnectionCache,
                groupedConnection.Key,
                GroupedConnectionKeyLevel.DatabaseId,
                dispatcherScheduler,
                groupedCn =>
                    new DatabaseNode(this, groupedCn, managementActionsController, explicitConnectionCache,
                        implicitConnectionCache, dispatcherScheduler), out nodes);
            Databases = nodes;
        }

        public ICommand CreateDatabaseCommand { get; }

        public string Host { get; }

        public string AuthorisationKeyHint { get; }

        public string AuthorisationKey { get; }

        public IEnumerable<DatabaseNode> Databases { get; }        

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }

    public class DatabaseNode : IDisposable
    {        
        private readonly IDisposable _disposable;

        public DatabaseNode(HostNode owner, GroupedConnection groupedConnection, IManagementActionsController managementActionsController, IExplicitConnectionCache explicitConnectionCache, IImplicitConnectionCache implicitConnectionCache, DispatcherScheduler dispatcherScheduler)
        {
            Owner = owner;
            ReadOnlyObservableCollection<CollectionNode> nodes;
            _disposable = explicitConnectionCache.BuildChildNodes(
                implicitConnectionCache,
                groupedConnection.Key,
                GroupedConnectionKeyLevel.CollectionId,
                dispatcherScheduler,
                groupedCn =>
                    new CollectionNode(this, groupedCn[GroupedConnectionKeyLevel.CollectionId], managementActionsController), 
                out nodes);
            Collections = nodes;

            DatabaseId = groupedConnection[GroupedConnectionKeyLevel.DatabaseId];

            CreateCollectionCommand = new Command(_ => managementActionsController.AddCollection(this));
            DeleteDatabaseCommand = new Command(_ => managementActionsController.DeleteDatabase(this));
        }

        public HostNode Owner { get; }

        public ICommand DeleteDatabaseCommand { get; }

        public ICommand CreateCollectionCommand { get; }        

        public string DatabaseId { get; }

        public IEnumerable<CollectionNode> Collections { get; }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }

    public class CollectionNode
    {
        public CollectionNode(DatabaseNode owner, string collectionId, IManagementActionsController managementActionsController)
        {
            Owner = owner;
            CollectionId = collectionId;

            DeleteCollectionCommand = new Command(_ => managementActionsController.DeleteCollection(this));
        }

        public ICommand DeleteCollectionCommand { get; }

        public DatabaseNode Owner { get; }

        public string CollectionId { get; }
    }
}
