using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Doobry.Settings;
using MaterialDesignThemes.Wpf;

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

        public static IDisposable LaunchGettingStarted(
            IObservable<LocalEmulatorDetectorUnit> detectorUnitObservable,
            ISnackbarMessageQueue snackbarMessageQueue)
        {
            return detectorUnitObservable
                .Select(u => u.IsRunnng && u.Connections.All(cn => cn.DatabaseId == null && cn.CollectionId == null))
                .StartWith(false)
                .DistinctUntilChanged()
                .Where(
                    isLocalEmulatorRunningWithoutDatabasesAndCollections =>
                            isLocalEmulatorRunningWithoutDatabasesAndCollections)
                .Subscribe(_ => snackbarMessageQueue.Enqueue("Local Emulator Detected"));
        }
    }
}