using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Doobry.Settings;
using MaterialDesignThemes.Wpf;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Doobry
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private Connection _connection;
        private Settings.GeneralSettings _generalSettings;
        private string _documentId;
        //TODO delete
        private ResultSet _resultSet;
        private string _query;        


        public MainWindowViewModel()
        {            
            _generalSettings = new Settings.GeneralSettings(10);

            StartupCommand = new Command(_ => RunStartup());
            ShutDownCommand = new Command(_ => RunShutdown());
            FetchDocumentCommand = new Command(o => RunQueryAsync($"SELECT * FROM root r WHERE r.id = '{DocumentId}'")); 
            RunQueryCommand = new Command(o => RunQueryAsync(Query));
            EditConnectionCommand = new Command(o => EditConnectionAsync());
            EditSettingsCommand = new Command(o => EditSettingsAsync());
            ResultSetExplorer = new ResultSetExplorerViewModel();
        }

        public string DocumentId
        {
            get { return _documentId; }
            set { this.MutateVerbose(ref _documentId, value, RaisePropertyChanged()); }
        }

        public string Query
        {
            get { return _query; }
            set { this.MutateVerbose(ref _query, value, RaisePropertyChanged()); }
        }

        public ResultSet ResultSet
        {
            get { return _resultSet; }
            private set { this.MutateVerbose(ref _resultSet, value, RaisePropertyChanged()); }
        }

        public ICommand StartupCommand { get; }

        public ICommand ShutDownCommand { get; }

        public ICommand FetchDocumentCommand { get; }

        public ICommand EditConnectionCommand { get; }

        public ICommand EditSettingsCommand { get; }

        public ICommand RunQueryCommand { get; }

        public ResultSetExplorerViewModel ResultSetExplorer { get; }


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

        private async void RunQueryAsync(string query)
        {
            //TODO disable commands
            if (query == null || _connection == null) return;

            await DialogHost.Show(new ProgressRing(), delegate (object sender, DialogOpenedEventArgs args)
            {
                Task<ResultSet>.Factory.StartNew(() => RunQuery(query)).ContinueWith(task =>
                {
                    ResultSetExplorer.ResultSet = task.Result;
                    args.Session.Close();
                }, TaskScheduler.FromCurrentSynchronizationContext());
            });
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
            var connectionEditor = new ConnectionEditor
            {
                DataContext = viewModel
            };

            var result = await ShowDialogAsync(connectionEditor, "Database", PackIconKind.Database);

            if (result)
            {
                _connection = new Connection(viewModel.Host, viewModel.AuthorisationKey, viewModel.DatabaseId, viewModel.CollectionId);
                PersistSettings(_generalSettings, _connection);
            }
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

            if (result)
            {
                _generalSettings = new GeneralSettings(viewModel.MaxItemCount);
                PersistSettings(_generalSettings, _connection);
            }
        }

        private void PersistSettings(GeneralSettings generalSettings, Connection connection)
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

        private ResultSet RunQuery(string query)
        {
            using (var documentClient = new DocumentClient(new Uri(_connection.Host), _connection.AuthorisationKey))
            {
                try
                {
                    var feedOptions = new FeedOptions { MaxItemCount = _generalSettings.MaxItemCount };
                    var documentQuery = documentClient.CreateDocumentQuery(
                        UriFactory.CreateDocumentCollectionUri(_connection.DatabaseId, _connection.CollectionId), query, feedOptions);

                    var resultSet = new ResultSet(documentQuery.AsEnumerable().Select((dy, row) => new Result(row, dy.ToString())));
                    
                    return resultSet;
                }
                catch (DocumentClientException de)
                {
                    var baseException = de.GetBaseException();
                    return new ResultSet(baseException.Message);
                }
                catch (Exception e)
                {
                    var baseException = e.GetBaseException();
                    return new ResultSet(baseException.Message);
                }
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}
