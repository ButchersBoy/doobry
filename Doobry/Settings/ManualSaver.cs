namespace Doobry.Settings
{
    public class ManualSaver : IManualSaver
    {
        private readonly object _gate = new object();

        public void Save(IExplicitConnectionCache explicitConnectionCache, IGeneralSettings generalSettings)
        {
            lock (_gate)
            {
                var layoutStructure = LayoutAnalayzer.GetLayoutStructure();
                var data = Serializer.Stringify(explicitConnectionCache, generalSettings, layoutStructure);

                new Persistance().TrySaveRaw(data);
            }
        }
    }
}