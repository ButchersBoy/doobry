using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace DocumentDbIx
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private Connection _connection;
        private Settings _settings;
        private string _documentId;
        private string _result;
        private string _query;        

        /*
config.host = process.env.HOST || "https://c64.documents.azure.com:443/";
config.authKey = process.env.AUTH_KEY || "BEZfZWprSG5a6SVMCXvNJClfx3Nchk7sASMX6S8LRnCSSeh5dPdPM9657jsQHqdaMlOjMlZCw1eaMOzCWCgCNg==";
config.databaseId = "quotesystem";
config.collectionId = "california";
         */

        public MainWindowViewModel()
        {
            _connection = new Connection(
                "https://c64.documents.azure.com:443/",
                "BEZfZWprSG5a6SVMCXvNJClfx3Nchk7sASMX6S8LRnCSSeh5dPdPM9657jsQHqdaMlOjMlZCw1eaMOzCWCgCNg==",
                "quotesystem", 
                "california");
            _settings = new Settings(10);

            FetchDocumentCommand = new Command(o => RunQueryAsync($"SELECT * FROM root r WHERE r.id = '{DocumentId}'")); 
            RunQueryCommand = new Command(o => RunQueryAsync(Query));
            EditConnectionCommand = new Command(o => EditConnectionAsync());
            EditSettingsCommand = new Command(o => EditSettingsAsync());
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

        public string Result
        {
            get { return _result; }
            private set { this.MutateVerbose(ref _result, value, RaisePropertyChanged()); }
        }

        public ICommand FetchDocumentCommand { get; }

        public ICommand EditConnectionCommand { get; }

        public ICommand EditSettingsCommand { get; }

        public ICommand RunQueryCommand { get; }

        private async void RunQueryAsync(string query)
        {
            await DialogHost.Show(new ProgressRing(), delegate (object sender, DialogOpenedEventArgs args)
            {
                Task<string>.Factory.StartNew(() => RunQuery(query)).ContinueWith(task =>
                {
                    Result = task.Result;
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

                    var resultSet = new ResultSet(documentQuery.AsEnumerable().Select((dy, row) => new Result(row, dy.ToString()));
                    
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

    public class Result
    {
        public Result(int row, string data)
        {
            Row = row;
            Data = data;
        }

        public int Row { get; }

        public string Data { get; }
    }

    public class ResultSet
    {
        public ResultSet(IEnumerable<Result> results)
        {
            if (results == null) throw new ArgumentNullException(nameof(results));

            Results = new ReadOnlyCollectionBuilder<Result>(results).ToReadOnlyCollection();
        }

        public ResultSet(string error)
        {
            Error = error;
        }

        public string Error { get; }


        public IReadOnlyCollection<Result> Results { get; }
    }
}
