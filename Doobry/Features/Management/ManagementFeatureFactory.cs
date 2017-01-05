using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
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
        private readonly DispatcherScheduler _dispatcherScheduler;

        public ManagementFeatureFactory(IExplicitConnectionCache explicitConnectionCache, IImplicitConnectionCache implicitConnectionCache, DispatcherScheduler dispatcherScheduler)
        {
            if (explicitConnectionCache == null) throw new ArgumentNullException(nameof(explicitConnectionCache));
            if (implicitConnectionCache == null) throw new ArgumentNullException(nameof(implicitConnectionCache));
            if (dispatcherScheduler == null) throw new ArgumentNullException(nameof(dispatcherScheduler));

            _explicitConnectionCache = explicitConnectionCache;
            _implicitConnectionCache = implicitConnectionCache;
            _dispatcherScheduler = dispatcherScheduler;
            FeatureId = new Guid("65079BF1-EEAE-45BA-A7E1-9A66D3EEA892");
        }

        public void WriteToBackingStore(object tabContentViewModel, JToken @into)
        {
        }

        public Guid FeatureId { get; }

        public ITabContentLifetimeHost CreateTabContent()
        {
            var managementViewModel = new ManagementViewModel(_explicitConnectionCache, _implicitConnectionCache, _dispatcherScheduler);
            return new TabContentLifetimeHost(managementViewModel, reason => managementViewModel.Dispose());
        }

        public ITabContentLifetimeHost RestoreTabContent(LayoutStructureTabItem tabItem)
        {
            var managementViewModel = new ManagementViewModel(_explicitConnectionCache, _implicitConnectionCache, _dispatcherScheduler);
            return new TabContentLifetimeHost(managementViewModel, reason => managementViewModel.Dispose());
        }
    }
}
