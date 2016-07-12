using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Doobry.Infrastructure;
using Doobry.Settings;
using MaterialDesignThemes.Wpf;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Doobry
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private Connection _connection;
        private GeneralSettings _generalSettings;
        private string _documentId;        

        public MainWindowViewModel()
        {            
            _generalSettings = new GeneralSettings(10);

            StartupCommand = new Command(_ => RunStartup());
            ShutDownCommand = new Command(_ => RunShutdown());
            FetchDocumentCommand = new Command(o => QueryRunnerViewModel.Run($"SELECT * FROM root r WHERE r.id = '{DocumentId}'"));             
            EditConnectionCommand = new Command(o => EditConnectionAsync());
            EditSettingsCommand = new Command(o => EditSettingsAsync());            
            QueryRunnerViewModel = new QueryRunnerViewModel(() => _connection, () => _generalSettings);
            DocumentEditorViewModel = new DocumentEditorViewModel(() => _connection);
        }

        public string DocumentId
        {
            get { return _documentId; }
            set { this.MutateVerbose(ref _documentId, value, RaisePropertyChanged()); }
        }

        public ICommand StartupCommand { get; }

        public ICommand ShutDownCommand { get; }

        public ICommand FetchDocumentCommand { get; }

        public ICommand EditConnectionCommand { get; }

        public ICommand EditSettingsCommand { get; }        

        public QueryRunnerViewModel QueryRunnerViewModel { get; }

        public DocumentEditorViewModel DocumentEditorViewModel { get; }

        private void RunStartup()
        {
            string rawData;
            if (new Persistance().TryLoadRaw(out rawData))
            {
                try
                {
                    var tuple = Serializer.Objectify(rawData);
                    _connection = tuple.Item1;
                    _generalSettings = tuple.Item2;
                }
                catch
                {
                    //TODO summit...                    
                }                
            }

            if (_connection == null)
                EditConnectionAsync();
        }

        private void RunShutdown()
        {
            //Serializer.
        }

        

        private async void EditConnectionAsync()
        {
            var viewModel = new ConnectionEditorViewModel();
            if (_connection != null)
            {
                viewModel.Host = _connection.Host;
                viewModel.AuthorisationKey = _connection.AuthorisationKey;
                viewModel.CollectionId = _connection.CollectionId;
                viewModel.DatabaseId = _connection.DatabaseId;
            }
            var connectionsEditor = new ConnectionsEditor
            {
                DataContext = viewModel
            };

            var result = await ShowDialogAsync(connectionsEditor, "Database", PackIconKind.Database);

            if (!result) return;

            _connection = new Connection(viewModel.Label, viewModel.Host, viewModel.AuthorisationKey, viewModel.DatabaseId, viewModel.CollectionId);
            PersistSettings(_generalSettings, _connection);
        }

        private async void EditSettingsAsync()
        {
            var viewModel = new GeneralSettingsEditorViewModel
            {
                MaxItemCount = _generalSettings.MaxItemCount
            };
            var settingsEditor = new GeneralSettingsEditor
            {
                DataContext = viewModel
            };

            var result = await ShowDialogAsync(settingsEditor, "Settings", PackIconKind.Settings);

            if (!result) return;

            _generalSettings = new GeneralSettings(viewModel.MaxItemCount);
            PersistSettings(_generalSettings, _connection);
        }

        private static void PersistSettings(GeneralSettings generalSettings, Connection connection)
        {
            Task.Factory.StartNew(() =>
            {
                //TODO, handle errors, failures
                var json = Serializer.Stringify(connection, generalSettings);

                new Persistance().TrySaveRaw(json);
            });
        }

        private static async Task<bool> ShowDialogAsync(object content, string title, PackIconKind icon)
        {
            var dialogContentControl = new DialogContentControl
            {                
                Content = content,
                Title = title,
                Icon = icon
            };

            var result = await DialogHost.Show(dialogContentControl);

            return bool.TrueString.Equals(result);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}
