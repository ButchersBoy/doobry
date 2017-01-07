using System;
using Doobry.Settings;

namespace Doobry.DocumentDb
{
    public static class LocalEmulatorActions
    {
        public static IDisposable MergeConnectionsIntoCache(
            IObservable<LocalEmulatorDetectorUnit> detectorUnitObservable,
            IImplicitConnectionCache implicitConnectionCache)
        {
            return detectorUnitObservable.Subscribe(unit => implicitConnectionCache.Merge("LocalEmulator", unit.Connections));
        }
    }
}