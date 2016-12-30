using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Doobry.Features;
using Dragablz;

namespace Doobry.Infrastructure
{
    public class WindowInstanceManager : IWindowInstanceManager
    {
        private readonly Func<MainWindowViewModel> _mainWindowViewModelFactory;
        
        public WindowInstanceManager(Func<MainWindowViewModel> mainWindowViewModelFactory)
        {
            if (mainWindowViewModelFactory == null) throw new ArgumentNullException(nameof(mainWindowViewModelFactory));

            _mainWindowViewModelFactory = mainWindowViewModelFactory;
        }

        public MainWindow Create()
        {
            var result = new MainWindow
            {
                DataContext = _mainWindowViewModelFactory()
            };
            
            result.Closed += WindowOnClosed;
            return result;
        }

        private static void WindowOnClosed(object sender, EventArgs eventArgs)
        {
            var window = (Window)sender;
            window.Closed -= WindowOnClosed;

            var redundantTabs = TabablzControl.GetLoadedInstances()
                .SelectMany(
                    tabControl =>
                        tabControl.Items.OfType<TabItemContainer>()
                            .Select(tabItemContainer => new { tabControl, tabViewModel = tabItemContainer }))
                .Where(a => window.Equals(Window.GetWindow(a.tabControl)))
                .ToList();

            //TODO this is shoddy, but am in a rush right now.  What if user kills app from task bar? or OS shut down
            var remainingWindowCount = Application.Current.Windows.OfType<MainWindow>().Count();
            var tabCloseReason = remainingWindowCount == 0 ? TabCloseReason.ApplicationClosed : TabCloseReason.TabClosed;

            foreach (var redundantTab in redundantTabs)
            {
                redundantTab.tabViewModel.TabContentLifetimeHost.Cleanup(tabCloseReason);
            }
        }
    }
}