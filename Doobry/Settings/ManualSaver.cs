namespace Doobry.Settings
{
    public class ManualSaver : IManualSaver
    {
        private readonly object _gate = new object();

        public void Save(IConnectionCache connectionCache, IGeneralSettings generalSettings)
        {
            lock (_gate)
            {
                var layoutStructure = LayoutAnalayzer.GetLayoutStructure();
                var data = Serializer.Stringify(connectionCache, generalSettings, layoutStructure);

                new Persistance().TrySaveRaw(data);
            }
        }
    }
}