using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Doobry.Infrastructure;
using Doobry.Settings;
using ICSharpCode.AvalonEdit.Document;
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
        private const string SampleContent = "{ hello : \"world\" }";

        public DocumentEditorViewModel(Func<Connection> connectionProvider, ISnackbarMessageQueue snackbarMessageQueue)
        {
            if (connectionProvider == null) throw new ArgumentNullException(nameof(connectionProvider));

            _connectionProvider = connectionProvider;
            SaveCommand = new Command(_ => Save(snackbarMessageQueue));
            NewCommand = new Command(_ => Document.Text = SampleContent);

            Document = new TextDocument();
        }

        public TextDocument Document { get; }

        public ICommand SaveCommand { get; }

        public ICommand NewCommand { get; }

        private void Save(ISnackbarMessageQueue snackbarMessageQueue)
        {
            SaveAsync(snackbarMessageQueue);
        }

        private async void SaveAsync(ISnackbarMessageQueue snackbarMessageQueue)
        {
            var connection = _connectionProvider();

            //TODO disable commands
            if (string.IsNullOrWhiteSpace(Document.Text) || connection == null) return;

            await DialogHost.Show(new ProgressRing(), async delegate(object sender, DialogOpenedEventArgs args)
            {
                try
                {
                    var response = await Save(connection, Document.Text);

                    var jToken = JToken.Parse(response.Resource.ToString());
                    Document.Text = jToken.ToString(Formatting.Indented);

                    args.Session.Close();

                    snackbarMessageQueue.Enqueue($"Saved document '{response.Resource.Id}'.");
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