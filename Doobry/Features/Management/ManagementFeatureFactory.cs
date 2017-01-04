using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Doobry.Settings;
using Newtonsoft.Json.Linq;

namespace Doobry.Features.Management
{
    public class ManagementFeatureFactory : IFeatureFactory
    {
        private readonly IExplicitConnectionCache _explicitConnectionCache;
        private readonly IImplicitConnectionCache _implicitConnectionCache;

        public ManagementFeatureFactory(IExplicitConnectionCache explicitConnectionCache, IImplicitConnectionCache implicitConnectionCache)
        {
            if (explicitConnectionCache == null) throw new ArgumentNullException(nameof(explicitConnectionCache));
            if (implicitConnectionCache == null) throw new ArgumentNullException(nameof(implicitConnectionCache));

            _explicitConnectionCache = explicitConnectionCache;
            _implicitConnectionCache = implicitConnectionCache;
            FeatureId = new Guid("65079BF1-EEAE-45BA-A7E1-9A66D3EEA892");
        }

        public void WriteToBackingStore(object tabContentViewModel, JToken @into)
        {
        }

        public Guid FeatureId { get; }

        public ITabContentLifetimeHost CreateTabContent()
        {
            var managementViewModel = new ManagementViewModel(_explicitConnectionCache, _implicitConnectionCache);
            return new TabContentLifetimeHost(managementViewModel, reason => managementViewModel.Dispose());
        }

        public ITabContentLifetimeHost RestoreTabContent(LayoutStructureTabItem tabItem)
        {
            var managementViewModel = new ManagementViewModel(_explicitConnectionCache, _implicitConnectionCache);
            return new TabContentLifetimeHost(managementViewModel, reason => managementViewModel.Dispose());
        }
    }
}
