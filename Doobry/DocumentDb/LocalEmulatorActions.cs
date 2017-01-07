using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Xml.XPath;
using Doobry.Features.Management;
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
            ISnackbarMessageQueue snackbarMessageQueue,
            IManagementActionsController managementActionsController)
        {
            return detectorUnitObservable
                .Select(u => u.IsRunnng && u.Connections.All(cn => cn.DatabaseId == null && cn.CollectionId == null))
                .StartWith(false)
                .DistinctUntilChanged()
                .Where(
                    isLocalEmulatorRunningWithoutDatabasesAndCollections =>
                            isLocalEmulatorRunningWithoutDatabasesAndCollections)
                .Subscribe(
                    _ =>
                        snackbarMessageQueue.Enqueue("Local Emulator Detected", "GET STARTED",
                            async () => await RunGetStarted(managementActionsController)));
        }

        private static async Task RunGetStarted(IManagementActionsController managementActionsController)
        {
            var result = await managementActionsController.AddDatabase(LocalEmulator.Host, LocalEmulator.AuthorisationKey);
            if (!result.IsCompleted) return;
            await managementActionsController.AddCollection(LocalEmulator.Host, LocalEmulator.AuthorisationKey, result.ItemId);
        }
    }
}