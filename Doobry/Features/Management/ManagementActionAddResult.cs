namespace Doobry.Features.Management
{
    public class ManagementActionAddResult
    {
        private ManagementActionAddResult(string itemId = null)
        {
            IsCompleted = !string.IsNullOrEmpty(itemId);
            ItemId = itemId;
        }

        public static ManagementActionAddResult Complete(string itemId)
        {
            return new ManagementActionAddResult(itemId);
        }

        public static ManagementActionAddResult Incomplete()
        {
            return new ManagementActionAddResult();
        }

        public bool IsCompleted { get; }

        public string ItemId { get; }
    }
}