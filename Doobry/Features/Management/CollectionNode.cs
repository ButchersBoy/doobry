using System.Windows.Input;
using Doobry.Infrastructure;

namespace Doobry.Features.Management
{
    public class CollectionNode
    {
        public CollectionNode(DatabaseNode owner, string collectionId, IManagementActionsController managementActionsController)
        {
            Owner = owner;
            CollectionId = collectionId;

            DeleteCollectionCommand = new Command(_ => managementActionsController.DeleteCollection(this));
        }

        public ICommand DeleteCollectionCommand { get; }

        public DatabaseNode Owner { get; }

        public string CollectionId { get; }
    }
}