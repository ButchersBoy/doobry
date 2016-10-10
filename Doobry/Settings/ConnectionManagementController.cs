using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DynamicData.Kernel;
using MaterialDesignThemes.Wpf;

namespace Doobry.Settings
{
    public class ConnectionManagementController
    {
        private readonly IConnectionCache _connectionCache;

        public ConnectionManagementController(IConnectionCache connectionCache)
        {
            if (connectionCache == null) throw new ArgumentNullException(nameof(connectionCache));

            _connectionCache = connectionCache;
        }

        public async Task<Optional<Connection>> Select(DependencyObject nearTo)
        {
            if (nearTo == null) throw new ArgumentNullException(nameof(nearTo));

            Connection connection;

            if (!_connectionCache.Any())
            {
                var connectionEditor = new ConnectionEditor();
                connection = await nearTo.ShowDialog(connectionEditor, delegate(object sender, DialogOpenedEventArgs args)                
                {
                    var connectionEditorViewModel = new ConnectionEditorViewModel(vm => SaveHandler(vm, args.Session), args.Session.Close);
                    connectionEditor.DataContext = connectionEditorViewModel;
                }) as Connection;

                return Optional<Connection>.Create(connection);
            }

            var connectionsManager = new ConnectionsManager();
            connection = await nearTo.ShowDialog(connectionsManager, delegate(object sender, DialogOpenedEventArgs args)
            {
                var connectionsManagerViewModel = new ConnectionsManagerViewModel(_connectionCache);
                connectionsManager.DataContext = connectionsManagerViewModel;
            }) as Connection;

            return Optional<Connection>.Create(connection);
        }

        

        private void SaveHandler(ConnectionEditorViewModel connectionEditorViewModel, DialogSession dialogSession)
        {
            var connection = new Connection(connectionEditorViewModel.Id.GetValueOrDefault(Guid.NewGuid()),
                connectionEditorViewModel.Label, connectionEditorViewModel.Host,
                connectionEditorViewModel.AuthorisationKey, connectionEditorViewModel.DatabaseId,
                connectionEditorViewModel.CollectionId);
            _connectionCache.AddOrUpdate(connection);            
            dialogSession.Close(connection);            
        }
    }
}