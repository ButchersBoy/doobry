using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Doobry.Settings;

namespace Doobry.Features
{
    public interface IFeatureFactory : IBackingStoreWriter
    {
        Guid FeatureId { get; }

        ITabContentLifetimeHost CreateTabContent();

        ITabContentLifetimeHost RestoreTabContent(LayoutStructureTabItem tabItem);        
    }
}