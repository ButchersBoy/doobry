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
        private readonly IConnectionCache _connectionCache;

        public ManagementFeatureFactory(IConnectionCache connectionCache)
        {
            _connectionCache = connectionCache;
            FeatureId = new Guid("65079BF1-EEAE-45BA-A7E1-9A66D3EEA892");
        }

        public void WriteToBackingStore(object tabContentViewModel, JToken @into)
        {
        }

        public Guid FeatureId { get; }

        public ITabContentLifetimeHost CreateTabContent()
        {
            var managementViewModel = new ManagementViewModel(_connectionCache);
            return new TabContentLifetimeHost(managementViewModel, reason => managementViewModel.Dispose());
        }

        public ITabContentLifetimeHost RestoreTabContent(LayoutStructureTabItem tabItem)
        {
            var managementViewModel = new ManagementViewModel(_connectionCache);
            return new TabContentLifetimeHost(managementViewModel, reason => managementViewModel.Dispose());
        }
    }
}
