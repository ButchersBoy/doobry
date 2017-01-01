using System;

namespace Doobry.Features
{
    public class TabContentLifetimeHost : ITabContentLifetimeHost
    {
        private readonly Action<TabCloseReason> _cleanup;        

        public TabContentLifetimeHost(object viewModel, Action<TabCloseReason> cleanup)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
            if (cleanup == null) throw new ArgumentNullException(nameof(cleanup));

            _cleanup = cleanup;
            ViewModel = viewModel;            
        }

        public object ViewModel { get; }

        public void Cleanup(TabCloseReason closeReason)
        {
            _cleanup(closeReason);
        }
    }
}