using System;
using System.Collections.Generic;
using System.Linq;

namespace Doobry.Settings
{
    public class LayoutStructureTabSet
    {
        public LayoutStructureTabSet(Guid id, IEnumerable<LayoutStructureTabItem> tabItems)
        {
            Id = id;
            if (tabItems == null) throw new ArgumentNullException(nameof(tabItems));

            TabItems = tabItems.ToArray();
        }

        public Guid Id { get; }

        public IEnumerable<LayoutStructureTabItem> TabItems { get; }
    }
}