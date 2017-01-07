using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using System.Windows.Input;
using Doobry.Infrastructure;
using Doobry.Settings;

namespace Doobry.Features.Management
{
    public class DatabaseNode : IDisposable
    {
        private static readonly IComparer<CollectionNode> CollectionSortComparer =
            SortExpressionComparer<CollectionNode>.Ascending(cn => cn.CollectionId);

        private readonly IDisposable _disposable;

        public DatabaseNode(HostNode owner, GroupedConnection groupedConnection, IManagementActionsController managementActionsController, IExplicitConnectionCache explicitConnectionCache, IImplicitConnectionCache implicitConnectionCache, DispatcherScheduler dispatcherScheduler)
        {
            Owner = owner;
            ReadOnlyObservableCollection<CollectionNode> nodes;
            _disposable = explicitConnectionCache.BuildChildNodes(
                implicitConnectionCache,
                groupedConnection.Key,
                GroupedConnectionKeyLevel.CollectionId,
                groupedCn =>
                        new CollectionNode(this, groupedCn[GroupedConnectionKeyLevel.CollectionId], managementActionsController),
                CollectionSortComparer, 
                dispatcherScheduler, 
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
}