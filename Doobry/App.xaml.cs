using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Doobry.Infrastructure;
using Doobry.Settings;
using Dragablz;
using StructureMap;
using StructureMap.Pipeline;

namespace Doobry
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var container = new Container(_ =>
            {
                _.For<IConnectionCache>(Lifecycles.Singleton).Use<ConnectionCache>();
            });                      
        
            //grease the Dragablz wheels    
            NewItemFactory = () => new TabViewModel(container.GetInstance<IConnectionCache>());
            InterTabClient = new InterTabClient(container.GetInstance<MainWindowViewModel>);

            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            var mainWindow = new MainWindow
            {
                DataContext = container.GetInstance<MainWindowViewModel>()
            };
            mainWindow.Show();
        }

        public static Func<object> NewItemFactory { get; private set; }

        public static IInterTabClient InterTabClient { get; private set; }
    }

    /*
    public class EntryPoint
    {
        public static void Main()
    }
    */
}
