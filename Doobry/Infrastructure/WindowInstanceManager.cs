using System;

namespace Doobry.Infrastructure
{
    public class WindowInstanceManager : IWindowInstanceManager
    {
        private readonly ITabInstanceManager _tabInstanceManager;
        private readonly Func<MainWindowViewModel> _mainWindowViewModelFactory;

        public WindowInstanceManager(ITabInstanceManager tabInstanceManager, Func<MainWindowViewModel> mainWindowViewModelFactory)
        {
            if (tabInstanceManager == null) throw new ArgumentNullException(nameof(tabInstanceManager));
            if (mainWindowViewModelFactory == null) throw new ArgumentNullException(nameof(mainWindowViewModelFactory));

            _tabInstanceManager = tabInstanceManager;
            _mainWindowViewModelFactory = mainWindowViewModelFactory;
        }

        public MainWindow Create()
        {
            var result = new MainWindow
            {
                DataContext = _mainWindowViewModelFactory()
            };
            _tabInstanceManager.Manage(result);
            return result;
        }

    }
}