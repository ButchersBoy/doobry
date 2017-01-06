using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using System.Windows.Input;
using Doobry.Infrastructure;
using Doobry.Settings;

namespace Doobry.Features.Management
{
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
}