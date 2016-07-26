using System;

namespace Doobry.Settings
{
    public class LayoutStructureTabItem
    {
        public LayoutStructureTabItem(Guid? connectionId)
        {
            ConnectionId = connectionId;
        }

        public Guid? ConnectionId { get; }
    }
}