using System.Threading.Tasks;

namespace Doobry.Features.Management
{
    public interface IManagementActionsController
    {
        Task<ManagementActionAddResult> AddDatabase(HostNode host);

        Task<ManagementActionAddResult> AddDatabase(string host, string authorisationKey);

        Task<ManagementActionAddResult> AddCollection(DatabaseNode database);

        Task<ManagementActionAddResult> AddCollection(string host, string authorisationKey, string databaseId);

        Task<bool> DeleteDatabase(DatabaseNode database);

        Task<bool> DeleteDatabase(string host, string authorisationKey, string databaseId);

        Task<bool> DeleteCollection(CollectionNode collection);

        Task<bool> DeleteCollection(string host, string authorisationKey, string databaseId, string collectionId);
    }
}