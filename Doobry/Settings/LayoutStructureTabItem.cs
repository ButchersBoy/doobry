using System;
using Doobry.Features;
using Newtonsoft.Json.Linq;

namespace Doobry.Settings
{
    //TODO make readable and writable versions?
    public class LayoutStructureTabItem
    {
        private readonly JToken _tabItemJToken;

        public LayoutStructureTabItem(Guid id, Guid featureId, object tabContentViewModel, IBackingStoreWriter backingStoreWriter)
        {
            BackingStoreWriter = backingStoreWriter;            
            Id = id;
            FeatureId = featureId;
            TabContentViewModel = tabContentViewModel;
        }

        public LayoutStructureTabItem(Guid id, Guid featureId, JToken tabItemJToken)
        {
            _tabItemJToken = tabItemJToken;
            Id = id;
            FeatureId = featureId;
        }

        public Guid Id { get; }

        public Guid FeatureId { get; set; }

        public object TabContentViewModel { get; }

        public IBackingStoreWriter BackingStoreWriter { get; }

        public string ReadProperty(string name)
        {
            return _tabItemJToken.Value<string>(name);
        }

        //public Guid? ConnectionId { get; }
    }
}