namespace Doobry
{
    public class Settings
    {
        public Settings(int? maxItemCount)
        {
            MaxItemCount = maxItemCount;
        }

        public int? MaxItemCount { get; }
    }
}