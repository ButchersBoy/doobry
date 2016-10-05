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
        private readonly Func<MainWindowViewModel> _mainWindowViewModelFactory;

        public InterTabClient(Func<MainWindowViewModel> mainWindowViewModelFactory)
        {
            if (mainWindowViewModelFactory == null) throw new ArgumentNullException(nameof(mainWindowViewModelFactory));
            _mainWindowViewModelFactory = mainWindowViewModelFactory;
        }

        public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
        {
            var mainWindow = new MainWindow
            {
                DataContext = _mainWindowViewModelFactory()
            };

            return new NewTabHost<Window>(mainWindow, mainWindow.InitialTabablzControl);
        }

        public TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
        {
            return TabEmptiedResponse.CloseWindowOrLayoutBranch;
        }
    }
}
