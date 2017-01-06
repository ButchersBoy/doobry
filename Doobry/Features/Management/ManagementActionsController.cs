using System;
using System.CodeDom;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Doobry.Infrastructure;
using MaterialDesignThemes.Wpf;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Doobry.Features.Management
{
    public class ManagementActionsController : IManagementActionsController
    {
        private readonly IDialogTargetFinder _dialogTargetFinder;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;

        public ManagementActionsController(IDialogTargetFinder dialogTargetFinder, ISnackbarMessageQueue snackbarMessageQueue)
        {
            if (dialogTargetFinder == null) throw new ArgumentNullException(nameof(dialogTargetFinder));
            if (snackbarMessageQueue == null) throw new ArgumentNullException(nameof(snackbarMessageQueue));

            _dialogTargetFinder = dialogTargetFinder;
            _snackbarMessageQueue = snackbarMessageQueue;
        }

        public async void AddDatabase(HostNode host)
        {
            var createItemProperties = new CreateItemProperties("Database");
            await DoAction(
                createItemProperties,
                properties => DoAddDatabase(properties, host));            
        }

        public async void AddCollection(DatabaseNode database)
        {
            var createItemProperties = new CreateItemProperties("Collection");
            await DoAction(
                createItemProperties,
                properties => DoAddCollection(properties, database));
        }

        public async void DeleteDatabase(DatabaseNode database)
        {
            var deleteDatabaseProperties = new DeleteItemProperties("database", database.DatabaseId);
            await DoAction(
                deleteDatabaseProperties, 
                p => DoDeleteDatabase(database));
        }

        public async void DeleteCollection(CollectionNode collection)
        {
            var createItemProperties = new CreateItemProperties("Collection");
            await DoAction(
                createItemProperties,
                properties => DoDeleteCollection(collection));
        }

        private async Task DoAction<TProperties>(TProperties properties, Func<TProperties, Task> taskFactory) where TProperties : INotifyPropertyChanged, INotifyDataErrorInfo
        {
            var view = new ManagementAction();
            
            ManagementActionViewModel<TProperties> model = null;
            await DialogHost.Show(view, _dialogTargetFinder.SuggestDialogHostIdentifier(),
                delegate (object sender, DialogOpenedEventArgs args)
                {
                    model = new ManagementActionViewModel<TProperties>(properties, taskFactory, () => args.Session.Close());
                    view.DataContext = model;
                });
            model?.Dispose();
        }

        private static Task DoAddCollection(CreateItemProperties properties, DatabaseNode database)
        {
            var documentClient = CreateDocumentClient(database.Owner);
            var documentCollection = new DocumentCollection
            {
                Id = properties.ItemId
            };
            return documentClient.CreateDocumentCollectionAsync("dbs/" + database.DatabaseId, documentCollection);
        }

        private static Task DoDeleteCollection(CollectionNode collection)
        {
            var documentClient = CreateDocumentClient(collection.Owner.Owner);
            return documentClient.DeleteDocumentCollectionAsync(
                $"dbs/{collection.Owner.DatabaseId}/colls/{collection.CollectionId}");
        }

        private static Task DoDeleteDatabase(DatabaseNode databaseNode)
        {
            return CreateDocumentClient(databaseNode.Owner).DeleteDatabaseAsync("dbs/" + databaseNode.DatabaseId);
        }

        private static Task DoAddDatabase(CreateItemProperties properties, HostNode hostNode)
        {
            var database = new Database
            {
                Id = properties.ItemId
            };
            return CreateDocumentClient(hostNode).CreateDatabaseAsync(database);
        }

        private static DocumentClient CreateDocumentClient(HostNode hostNode)
        {
            return new DocumentClient(new Uri(hostNode.Host), hostNode.AuthorisationKey);
        }
    }
}