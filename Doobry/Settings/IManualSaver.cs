namespace Doobry.Settings
{
    public interface IManualSaver
    {
        void Save(IConnectionCache connectionCache, IGeneralSettings generalSettings);
    }
}