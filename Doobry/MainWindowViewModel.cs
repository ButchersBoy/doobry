using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Doobry
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private Connection _connection;
        private Settings _settings;
        private string _documentId;
        //TODO delete
        private ResultSet _resultSet;
        private string _query;        


        public MainWindowViewModel()
        {            
            _settings = new Settings(10);

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

        public ICommand FetchDocumentCommand { get; }

        public ICommand EditConnectionCommand { get; }

        public ICommand EditSettingsCommand { get; }

        public ICommand RunQueryCommand { get; }

        public ResultSetExplorerViewModel ResultSetExplorer { get; }

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
            }
        }

        private async void EditSettingsAsync()
        {
            var viewModel = new SettingsEditorViewModel
            {
                MaxItemCount = _settings.MaxItemCount
            };
            var settingsEditor = new SettingsEditor
            {
                DataContext = viewModel
            };

            var result = await ShowDialogAsync(settingsEditor, "Settings", PackIconKind.Settings);

            if (result)
            {
                _settings = new Settings(viewModel.MaxItemCount);
            }
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
                    var feedOptions = new FeedOptions { MaxItemCount = _settings.MaxItemCount };
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
