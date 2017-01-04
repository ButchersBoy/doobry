namespace Doobry.Settings
{
    public interface IManualSaver
    {
        void Save(IExplicitConnectionCache explicitConnectionCache, IGeneralSettings generalSettings);
    }
}