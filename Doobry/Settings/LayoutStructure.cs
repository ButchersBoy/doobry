using System;
using System.Collections.Generic;
using System.Linq;

namespace Doobry.Settings
{
    public class LayoutStructure
    {
        public LayoutStructure(IEnumerable<LayoutStructureWindow> windows)
        {
            if (windows == null) throw new ArgumentNullException(nameof(windows));

            Windows = windows.ToArray();
        }

        public IEnumerable<LayoutStructureWindow> Windows { get; }
    }
}