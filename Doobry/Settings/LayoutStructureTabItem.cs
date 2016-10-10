using System;

namespace Doobry.Settings
{
    public class LayoutStructureTabItem
    {
        public LayoutStructureTabItem(Guid id, Guid? connectionId)
        {
            Id = id;
            ConnectionId = connectionId;
        }

        public Guid Id { get; }

        public Guid? ConnectionId { get; }
    }
}