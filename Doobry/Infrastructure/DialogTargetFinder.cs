using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doobry.Infrastructure
{
    public class DialogTargetFinder : IDialogTargetFinder
    {
        public object SuggestDialogHostIdentifier()
        {
            return MainWindow.SuggestDialogHostIdentifier();
        }
    }
}
