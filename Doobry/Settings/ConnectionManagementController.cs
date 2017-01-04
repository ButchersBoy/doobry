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
        private readonly IExplicitConnectionCache _explicitConnectionCache;

        public ConnectionManagementController(IExplicitConnectionCache explicitConnectionCache)
        {
            if (explicitConnectionCache == null) throw new ArgumentNullException(nameof(explicitConnectionCache));

            _explicitConnectionCache = explicitConnectionCache;
        }

        public async Task<Optional<ExplicitConnection>> Select(DependencyObject nearTo)
        {
            if (nearTo == null) throw new ArgumentNullException(nameof(nearTo));

            ExplicitConnection explicitConnection;

            if (!_explicitConnectionCache.Any())
            {
                var connectionEditor = new ConnectionEditor();
                explicitConnection = await nearTo.ShowDialog(connectionEditor, delegate(object sender, DialogOpenedEventArgs args)                
                {
                    var connectionEditorViewModel = new ConnectionEditorViewModel(vm => SaveHandler(vm, args.Session), args.Session.Close);
                    connectionEditor.DataContext = connectionEditorViewModel;
                }) as ExplicitConnection;

                return Optional<ExplicitConnection>.Create(explicitConnection);
            }

            var connectionsManager = new ConnectionsManager();
            explicitConnection = await nearTo.ShowDialog(connectionsManager, delegate(object sender, DialogOpenedEventArgs args)
            {
                var connectionsManagerViewModel = new ConnectionsManagerViewModel(_explicitConnectionCache);
                connectionsManager.DataContext = connectionsManagerViewModel;
            }) as ExplicitConnection;

            return Optional<ExplicitConnection>.Create(explicitConnection);
        }

        

        private void SaveHandler(ConnectionEditorViewModel connectionEditorViewModel, DialogSession dialogSession)
        {
            var connection = new ExplicitConnection(connectionEditorViewModel.Id.GetValueOrDefault(Guid.NewGuid()),
                connectionEditorViewModel.Label, connectionEditorViewModel.Host,
                connectionEditorViewModel.AuthorisationKey, connectionEditorViewModel.DatabaseId,
                connectionEditorViewModel.CollectionId);
            _explicitConnectionCache.AddOrUpdate(connection);            
            dialogSession.Close(connection);            
        }
    }
}