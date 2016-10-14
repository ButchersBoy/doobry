using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Dragablz;

namespace Doobry.Infrastructure
{
    public class InterTabClient : IInterTabClient
    {
        private readonly IWindowInstanceManager _windowInstanceManager;

        public InterTabClient(IWindowInstanceManager windowInstanceManager)
        {
            if (windowInstanceManager == null) throw new ArgumentNullException(nameof(windowInstanceManager));
            _windowInstanceManager = windowInstanceManager;
        }

        public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
        {
            var managedWindow = _windowInstanceManager.Create();

            return new NewTabHost<Window>(managedWindow, managedWindow.InitialTabablzControl);
        }

        public TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
        {
            return TabEmptiedResponse.CloseWindowOrLayoutBranch;
        }
    }
}
