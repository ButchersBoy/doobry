using System;
using System.Reactive.Disposables;
using Doobry.Settings;
using Newtonsoft.Json.Linq;

namespace Doobry.Features
{
    public interface IBackingStoreWriter
    {
        void WriteToBackingStore(object tabContentViewModel, JToken into);
    }

    public interface IFeatureFactory : IBackingStoreWriter
    {
        Guid FeatureId { get; }

        ITabContentLifetimeHost CreateTabContent();

        ITabContentLifetimeHost RestoreTabContent(LayoutStructureTabItem tabItem);        
    }

    public enum TabCloseReason
    {
        ApplicationClosed,
        TabClosed
    }

    public interface ITabContentLifetimeHost
    {
        object ViewModel { get; }

        void Cleanup(TabCloseReason closeReason);
    }

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