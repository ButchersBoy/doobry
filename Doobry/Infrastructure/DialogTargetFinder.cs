using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doobry.Infrastructure
{
    public static class DialogTargetFinder
    {
        public static object SuggestDialogHostIdentifier()
        {
            return MainWindow.SuggestDialogHostIdentifier();
        }
    }
}
