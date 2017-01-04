using System;

namespace Doobry.Settings
{
    public class SettingsContainer
    {
        public SettingsContainer(IExplicitConnectionCache explicitConnectionCache, GeneralSettings generalSettings, LayoutStructure layoutStructure)
        {
            if (explicitConnectionCache == null) throw new ArgumentNullException(nameof(explicitConnectionCache));
            if (generalSettings == null) throw new ArgumentNullException(nameof(generalSettings));

            ExplicitConnectionCache = explicitConnectionCache;
            GeneralSettings = generalSettings;
            LayoutStructure = layoutStructure;
        }

        public IExplicitConnectionCache ExplicitConnectionCache { get; }

        public GeneralSettings GeneralSettings { get; }

        public LayoutStructure LayoutStructure { get; }
    }
}