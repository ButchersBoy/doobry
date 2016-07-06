using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Doobry.Infrastructure;
using Doobry.Settings;
using MaterialDesignThemes.Wpf;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Doobry
{
    public class DocumentEditorViewModel : INotifyPropertyChanged
    {
        private readonly Func<Connection> _connectionProvider;
        private string _content = "{ hello : \"world\" }";

        public DocumentEditorViewModel(Func<Connection> connectionProvider)
        {
            this._connectionProvider = connectionProvider;
            SaveCommand = new Command(_ => Save());
        }

        public ICommand SaveCommand { get; }

        public string Content
        {
            get { return _content; }
            set { this.MutateVerbose(ref _content, value, RaisePropertyChanged()); }
        }
        private void Save()
        {
            SaveAsync();
        }

        private async void SaveAsync()
        {
            var connection = _connectionProvider();

            //TODO disable commands
            if (string.IsNullOrWhiteSpace(Content) || connection == null) return;

            await DialogHost.Show(new Infrastructure.ProgressRing(), async delegate(object sender, DialogOpenedEventArgs args)
            {
                try
                {
                    var response = await Save(connection, Content);

                    var jToken = JToken.Parse(response.Resource.ToString());
                    Content = jToken.ToString(Formatting.Indented);

                    args.Session.Close();
                }
                catch (Exception exc)
                {
                    args.Session.UpdateContent(new MessageDialog
                    {
                        Title = "Save Error",
                        Content = exc.Message
                    });
                }
            });
        }

        private static async Task<ResourceResponse<Document>> Save(Connection connection, string content)
        {
            var jObject = JObject.Parse(content);

            var idToken = jObject["id"];

            using (var documentClient = new DocumentClient(new Uri(connection.Host), connection.AuthorisationKey))
            {
                var documentCollectionUri = UriFactory.CreateDocumentCollectionUri(connection.DatabaseId, connection.CollectionId);

                if (idToken == null)
                {
                    return await documentClient.CreateDocumentAsync(documentCollectionUri, jObject);                    
                }
                return await documentClient.UpsertDocumentAsync(documentCollectionUri, jObject);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}