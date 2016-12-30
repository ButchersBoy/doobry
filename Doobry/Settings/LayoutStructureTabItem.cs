using System;
using Doobry.Features;
using Newtonsoft.Json.Linq;

namespace Doobry.Settings
{
    //TODO make readable and writable versions?
    public class LayoutStructureTabItem
    {
        private readonly JToken _tabItemJToken;

        /*
        public LayoutStructureTabItem(Guid id, Guid? connectionId)
        {
            Id = id;
            ConnectionId = connectionId;
        }
        */

        public LayoutStructureTabItem(Guid id, object tabContentViewModel, IBackingStoreWriter backingStoreWriter)
        {
            BackingStoreWriter = backingStoreWriter;            
            Id = id;
            TabContentViewModel = tabContentViewModel;
        }

        public LayoutStructureTabItem(Guid id, JToken tabItemJToken)
        {
            _tabItemJToken = tabItemJToken;
            Id = id;
        }

        public Guid Id { get; }

        public object TabContentViewModel { get; }

        public IBackingStoreWriter BackingStoreWriter { get; }

        public TResult ReadProperty<TResult>(string name)
        {
            return _tabItemJToken.Value<TResult>(name);
        }

        //public Guid? ConnectionId { get; }
    }
}