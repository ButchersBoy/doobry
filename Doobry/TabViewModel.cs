using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Doobry.Infrastructure;
using Doobry.Settings;
using DynamicData.Kernel;
using MaterialDesignThemes.Wpf;

namespace Doobry
{
    public class TabViewModel : INotifyPropertyChanged
    {
        private Connection _connection;
        private readonly IConnectionCache _connectionCache;
        private GeneralSettings _generalSettings;
        private int _viewIndex;
        private string _documentId;
        private string _name;

        public TabViewModel(IConnectionCache connectionCache) : this(null, connectionCache)
        {
            
        }

        public TabViewModel(Connection connection, IConnectionCache connectionCache)
        {
            if (connectionCache == null) throw new ArgumentNullException(nameof(connectionCache));

            _generalSettings = new GeneralSettings(10);
            _connection = connection;
            _connectionCache = connectionCache;

            FetchDocumentCommand = new Command(o => QueryRunnerViewModel.Run($"SELECT * FROM root r WHERE r.id = '{DocumentId}'"));
            EditConnectionCommand = new Command(sender => EditConnectionAsync((DependencyObject)sender));
            EditSettingsCommand = new Command(sender => EditSettingsAsync((DependencyObject)sender));
            QueryRunnerViewModel = new QueryRunnerViewModel(() => _connection, () => _generalSettings);
            DocumentEditorViewModel = new DocumentEditorViewModel(() => _connection);

            SetName();
        }

        public string Name
        {
            get { return _name; }
            private set { this.MutateVerbose(ref _name, value, RaisePropertyChanged()); }
        }

        public string DocumentId
        {
            get { return _documentId; }
            set { this.MutateVerbose(ref _documentId, value, RaisePropertyChanged()); }
        }

        public int ViewIndex
        {
            get { return _viewIndex; }
            set { this.MutateVerbose(ref _viewIndex, value, RaisePropertyChanged()); }
        }

        public ICommand FetchDocumentCommand { get; }

        public ICommand EditConnectionCommand { get; }

        public ICommand EditSettingsCommand { get; }

        public QueryRunnerViewModel QueryRunnerViewModel { get; }

        public DocumentEditorViewModel DocumentEditorViewModel { get; }

        public Guid? ConnectionId => _connection?.Id;

        private async void EditConnectionAsync(DependencyObject sender)
        {
            Debug.Assert(sender != null);

            var connectionOption = await new ConnectionManagementController(_connectionCache).Select(sender);

            _connection = connectionOption.ValueOr(() => null);


            /*
            var connectionsManagerViewModel = new ConnectionsManagerViewModel(_connectionCache);
            var connectionsManager = new ConnectionsManager
            {
                DataContext = connectionsManagerViewModel
            };

            await ShowDialogAsync(connectionsManager, "Database", PackIconKind.Database, sender);

            /*
            ConnectionEditorViewModel selectConnectionEditorViewModel = null;
            var connectionEditorViewModels = MainWindowViewModel.ConnectionCache.Select(cn =>
            {
                var connectionEditorViewModel = cn.ToViewModel();
                if (_connection != null && connectionEditorViewModel.Id == _connection.Id)
                    selectConnectionEditorViewModel = connectionEditorViewModel;
                return connectionEditorViewModel;
            }).ToList();
            var connectionsEditorViewModel = new ConnectionsManagerViewModel(connectionEditorViewModels)
            {
                SelectedConnection = selectConnectionEditorViewModel
            };
            var connectionsEditor = new ConnectionsManager
            {
                DataContext = connectionsEditorViewModel
            };

            var result = await ShowDialogAsync(connectionsEditor, "Database", PackIconKind.Database, sender);

            if (!result) return;

            var connections = connectionsEditorViewModel.Connections.Select(cnVm =>
            {
                var cn = cnVm.ToConnection();
                if (cnVm == connectionsEditorViewModel.SelectedConnection)
                    SetConnection(cn);
                return cn;
            }).ToList();            

            MainWindowViewModel.ConnectionCache.Reset(connections);

            PersistSettings(_generalSettings, _connection);
            */
        }

        private async void EditSettingsAsync(DependencyObject sender)
        {
            var viewModel = new GeneralSettingsEditorViewModel
            {
                MaxItemCount = _generalSettings.MaxItemCount
            };
            var settingsEditor = new GeneralSettingsEditor
            {
                DataContext = viewModel
            };

            var result = await ShowDialogAsync(settingsEditor, "Settings", PackIconKind.Settings, sender);

            if (!result) return;

            _generalSettings = new GeneralSettings(viewModel.MaxItemCount);
            PersistSettings(_generalSettings, _connection);
        }

        private void SetConnection(Connection connection)
        {
            _connection = connection;
            SetName();
        }

        private void SetName()
        {
            Name = _connection?.Label ?? "(no connection)";
        }

        private static void PersistSettings(GeneralSettings generalSettings, Connection connection)
        {
            Task.Factory.StartNew(() =>
            {
                //TODO, handle errors, failures
                //var json = Serializer.Stringify( connection, generalSettings);

                //new Persistance().TrySaveRaw(json);
            });
        }

        private static async Task<bool> ShowDialogAsync(object content, string title, PackIconKind icon, DependencyObject sender)
        {
            var dialogContentControl = new DialogContentControl
            {
                Content = content,
                Title = title,
                Icon = icon,
                ShowStandardButtons = false
            };
            
            var result = await (sender == null ? DialogHost.Show(dialogContentControl) : sender.ShowDialog(dialogContentControl));

            return bool.TrueString.Equals(result);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}