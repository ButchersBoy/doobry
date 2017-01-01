using System;
using Doobry.Features;

namespace Doobry
{
    public class TabItemContainer
    {
        public TabItemContainer(Guid tabId, Guid featureId, ITabContentLifetimeHost tabContentLifetimeHost, IBackingStoreWriter backingStoreWriter)
        {
            TabId = tabId;
            FeatureId = featureId;
            TabContentLifetimeHost = tabContentLifetimeHost;
            BackingStoreWriter = backingStoreWriter;
            ViewModel = tabContentLifetimeHost.ViewModel;
            Name = "New World";
        }

        public string Name { get; }

        public Guid TabId { get; }

        public Guid FeatureId { get; }

        public ITabContentLifetimeHost TabContentLifetimeHost { get; }

        public IBackingStoreWriter BackingStoreWriter { get; }

        public object ViewModel { get; }
    }
}