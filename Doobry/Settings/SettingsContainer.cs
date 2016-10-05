using System;

namespace Doobry.Settings
{
    public class SettingsContainer
    {
        public SettingsContainer(IConnectionCache connectionCache, GeneralSettings generalSettings, LayoutStructure layoutStructure)
        {
            if (connectionCache == null) throw new ArgumentNullException(nameof(connectionCache));
            if (generalSettings == null) throw new ArgumentNullException(nameof(generalSettings));

            ConnectionCache = connectionCache;
            GeneralSettings = generalSettings;
            LayoutStructure = layoutStructure;
        }

        public IConnectionCache ConnectionCache { get; }

        public GeneralSettings GeneralSettings { get; }

        public LayoutStructure LayoutStructure { get; }
    }
}