﻿using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows;
using Doobry.DocumentDb;
using Doobry.Features;
using Doobry.Features.Management;
using Doobry.Features.QueryDeveloper;
using Doobry.Infrastructure;
using Doobry.Settings;
using Dragablz;
using MaterialDesignThemes.Wpf;
using StructureMap;
using Squirrel;

namespace Doobry
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Task<UpdateManager> _updateManager = null;

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            System.Net.WebRequest.DefaultWebProxy.Credentials
                = System.Net.CredentialCache.DefaultNetworkCredentials;

            IGeneralSettings generalSettings = null;
            IExplicitConnectionCache explicitConnectionCache = null;
            IInitialLayoutStructureProvider initialLayoutStructureProvider = null;            
            
            string rawData;
            if (new Persistance().TryLoadRaw(out rawData))
            {
                try
                {
                    var settingsContainer = Serializer.Objectify(rawData);
                    generalSettings = settingsContainer.GeneralSettings;
                    explicitConnectionCache = settingsContainer.ExplicitConnectionCache;
                    initialLayoutStructureProvider =
                        new InitialLayoutStructureProvider(settingsContainer.LayoutStructure);
                }
                catch (Exception exc)
                {
                    //TODO summit
                    System.Diagnostics.Debug.WriteLine(exc.Message);
                }
            }

            generalSettings = generalSettings ?? new GeneralSettings(10);
            explicitConnectionCache = explicitConnectionCache ?? new ExplicitConnectionCache();
            initialLayoutStructureProvider = initialLayoutStructureProvider ?? new InitialLayoutStructureProvider();

            var container = new Container(_ =>
            {
                _.ForSingletonOf<DispatcherScheduler>().Use(DispatcherScheduler.Current);
                _.ForSingletonOf<DispatcherTaskSchedulerProvider>().Use(DispatcherTaskSchedulerProvider.Create(Dispatcher));
                _.ForSingletonOf<IGeneralSettings>().Use(generalSettings);
                _.ForSingletonOf<IExplicitConnectionCache>().Use(explicitConnectionCache);
                _.ForSingletonOf<IImplicitConnectionCache>();
                _.ForSingletonOf<LocalEmulatorDetector>();
                _.ForSingletonOf<IInitialLayoutStructureProvider>().Use(initialLayoutStructureProvider);
                _.ForSingletonOf<ISnackbarMessageQueue>().Use(new SnackbarMessageQueue(TimeSpan.FromSeconds(5)));
                _.ForSingletonOf<FeatureRegistry>()
                    .Use(
                        ctx =>
                            FeatureRegistry
                                .WithDefault(ctx.GetInstance<QueryDeveloperFeatureFactory>())
                                .Add(ctx.GetInstance<ManagementFeatureFactory>()));
                _.AddRegistry<DoobryRegistry>();                
                _.Scan(scanner =>
                {
                    scanner.TheCallingAssembly();
                    scanner.WithDefaultConventions();
                });                
            });            

            var windowInstanceManager = new WindowInstanceManager(container.GetInstance<MainWindowViewModel>);

            //grease the Dragablz wheels    
            var featureRegistry = container.GetInstance<FeatureRegistry>();
            NewItemFactory = () =>
            {
                var contentLifetimeHost = featureRegistry.Default.CreateTabContent();
                var tabContentContainer = new TabItemContainer(Guid.NewGuid(), featureRegistry.Default.FeatureId, contentLifetimeHost, featureRegistry.Default);
                return tabContentContainer;
            };
            InterTabClient = new InterTabClient(windowInstanceManager);
            ClosingItemCallback = OnItemClosingHandler;

            var localEmulatorSubscription = UseLocalEmulatorDetector(container);
            Exit += (o, args) => localEmulatorSubscription.Dispose();

            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            var mainWindow = windowInstanceManager.Create();
            mainWindow.Show();
                
            Task.Factory.StartNew(() => CheckForUpdates(container.GetInstance<ISnackbarMessageQueue>()));
        }

        //easy access to stuff which dragablz needs 
        public static Func<object> NewItemFactory { get; private set; }
        public static IInterTabClient InterTabClient { get; private set; }
        public static ItemActionCallback ClosingItemCallback { get; private set; }

        private static async void CheckForUpdates(ISnackbarMessageQueue snackbarMessageQueue)
        {
            try
            {
                _updateManager = UpdateManager.GitHubUpdateManager("https://github.com/ButchersBoy/doobry", "doobry");

                if (_updateManager.Result.IsInstalledApp)
                    await _updateManager.Result.UpdateApp();
            }
            catch
            {
                snackbarMessageQueue.Enqueue("Unable to check for updates.");
            }
        }

        private static IDisposable UseLocalEmulatorDetector(IContainer container)
        {
            var localEmulatorDetector = container.GetInstance<LocalEmulatorDetector>();
            return new CompositeDisposable(
                LocalEmulatorActions.MergeConnectionsIntoCache(localEmulatorDetector, container.GetInstance<IImplicitConnectionCache>()),
                LocalEmulatorActions.LaunchGettingStarted(localEmulatorDetector, container.GetInstance<ISnackbarMessageQueue>(), container.GetInstance<IManagementActionsController>(), container.GetInstance<IExplicitConnectionCache>())
                );
        }

        private static void OnItemClosingHandler(ItemActionCallbackArgs<TabablzControl> args)
        {
            (args.DragablzItem.DataContext as TabItemContainer)?.TabContentLifetimeHost.Cleanup(TabCloseReason.TabClosed);
        }
    }
}
