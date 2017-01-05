using System;
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
            var view = new ManagementAction();            
            var createDatabaseProperties = new CreateDatabaseProperties();

            ManagementActionViewModel<CreateDatabaseProperties> model = null;
            await DialogHost.Show(view, _dialogTargetFinder.SuggestDialogHostIdentifier(),
                delegate(object sender, DialogOpenedEventArgs args)
                {
                    model = new ManagementActionViewModel<CreateDatabaseProperties>(createDatabaseProperties, p =>
                        DoAddDatabase(p, host), () => args.Session.Close());
                    view.DataContext = model;
                });
            model?.Dispose();
        }

        private static async void DoAddDatabase(CreateDatabaseProperties properties, HostNode hostNode)
        {
            var database = new Database
            {
                Id = properties.DatabaseId
            };
            await CreateDocumentClient(hostNode).CreateDatabaseAsync(database);
        }

        private static DocumentClient CreateDocumentClient(HostNode hostNode)
        {
            return new DocumentClient(new Uri(hostNode.Host), hostNode.AuthorisationKey);
        }
    }
}