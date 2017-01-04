namespace Doobry.Settings
{
    public interface IConnection
    {
        string Host { get; }
        string AuthorisationKey { get; }
        string DatabaseId { get; }
        string CollectionId { get; }
    }
}