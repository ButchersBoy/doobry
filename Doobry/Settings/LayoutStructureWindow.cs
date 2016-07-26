using System;
using System.Collections.Generic;
using System.Linq;

namespace Doobry.Settings
{
    public class LayoutStructureWindow
    {        
        public LayoutStructureWindow(IEnumerable<LayoutStructureBranch> branches, IEnumerable<LayoutStructureTabSet> tabSets)
        {
            if (branches == null) throw new ArgumentNullException(nameof(branches));
            if (tabSets == null) throw new ArgumentNullException(nameof(tabSets));

            Branches = branches.ToArray();
            TabSets = tabSets.ToArray();
        }

        public IEnumerable<LayoutStructureBranch> Branches { get; }

        public IEnumerable<LayoutStructureTabSet> TabSets { get; }
    }
}