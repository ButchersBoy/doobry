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
        public ManagementFeatureFactory()
        {
            FeatureId = new Guid("65079BF1-EEAE-45BA-A7E1-9A66D3EEA892");
        }

        public void WriteToBackingStore(object tabContentViewModel, JToken @into)
        {
        }

        public Guid FeatureId { get; }

        public ITabContentLifetimeHost CreateTabContent()
        {
            return new TabContentLifetimeHost(new ManagementViewModel(), reason => { });
        }

        public ITabContentLifetimeHost RestoreTabContent(LayoutStructureTabItem tabItem)
        {
            return new TabContentLifetimeHost(new ManagementViewModel(), reason => { });
        }
    }
}
