namespace Doobry.Settings
{
    public class GeneralSettings
    {
        public GeneralSettings(int? maxItemCount)
        {
            MaxItemCount = maxItemCount;
        }

        public int? MaxItemCount { get; }
    }
}