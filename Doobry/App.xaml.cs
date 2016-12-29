using System;
using System.Threading.Tasks;
using System.Windows;
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
            IConnectionCache connectionCache = null;
            IInitialLayoutStructureProvider initialLayoutStructureProvider = null;

            string rawData;
            if (new Persistance().TryLoadRaw(out rawData))
            {
                try
                {
                    var settingsContainer = Serializer.Objectify(rawData);
                    generalSettings = settingsContainer.GeneralSettings;
                    connectionCache = settingsContainer.ConnectionCache;
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
            connectionCache = connectionCache ?? new ConnectionCache();
            initialLayoutStructureProvider = initialLayoutStructureProvider ?? new InitialLayoutStructureProvider();

            var container = new Container(_ =>
            {
                _.ForSingletonOf<IGeneralSettings>().Use(generalSettings);
                _.ForSingletonOf<IConnectionCache>().Use(connectionCache);                
                _.ForSingletonOf<IInitialLayoutStructureProvider>().Use(initialLayoutStructureProvider);
                _.ForSingletonOf<ISnackbarMessageQueue>().Use(new SnackbarMessageQueue());
                _.AddRegistry<DoobryRegistry>();
                _.Scan(scanner =>
                {
                    scanner.TheCallingAssembly();
                    scanner.WithDefaultConventions();
                });                
            });

            var tabViewModelInstanceManager = container.GetInstance<ITabInstanceManager>();
            var windowInstanceManager = new WindowInstanceManager(tabViewModelInstanceManager, container.GetInstance<MainWindowViewModel>);
            
            //grease the Dragablz wheels    
            NewItemFactory = () => tabViewModelInstanceManager.CreateManagedTabViewModel();
            InterTabClient = new InterTabClient(windowInstanceManager);
            ClosingItemCallback = tabViewModelInstanceManager.ClosingTabItemCallback;

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
    }
}
