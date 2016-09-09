using System;

namespace Doobry.Settings
{
    public class SettingsContainer
    {
        public SettingsContainer(ConnectionCache connectionCache, GeneralSettings generalSettings, LayoutStructure layoutStructure)
        {
            if (connectionCache == null) throw new ArgumentNullException(nameof(connectionCache));
            if (generalSettings == null) throw new ArgumentNullException(nameof(generalSettings));

            ConnectionCache = connectionCache;
            GeneralSettings = generalSettings;
            LayoutStructure = layoutStructure;
        }

        public ConnectionCache ConnectionCache { get; }

        public GeneralSettings GeneralSettings { get; }

        public LayoutStructure LayoutStructure { get; }
    }
}