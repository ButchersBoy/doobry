namespace Doobry.Features.Management
{
    public interface IManagementActionsController
    {
        void AddDatabase(HostNode host);
        void DeleteDatabase(DatabaseNode database);
        void AddCollection(DatabaseNode database);
        void DeleteCollection(CollectionNode collection);
    }
}