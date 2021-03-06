﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using Doobry.Infrastructure;
using Doobry.Settings;

namespace Doobry.Features.Management
{
    public class ManagementViewModel : INamed, IDisposable
    {
        private static readonly IComparer<HostNode> HostSortComparer =
            SortExpressionComparer<HostNode>.Ascending(cn => cn.Host);

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
                groupedConnection =>
                    new HostNode(groupedConnection, managementActionsController, explicitConnectionCache,
                        implicitConnectionCache, dispatcherScheduler),
                HostSortComparer, 
                dispatcherScheduler, out nodes);
            Hosts = nodes;
        }

        public string Name { get; }

        public IEnumerable<HostNode> Hosts { get; }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
