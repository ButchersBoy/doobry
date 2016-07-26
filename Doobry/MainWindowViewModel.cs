using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Doobry.Infrastructure;
using Doobry.Settings;
using Dragablz;
using Dragablz.Dockablz;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Doobry
{
    public class MainWindowViewModel// : INotifyPropertyChanged
    {
        //TODO put these somewhere nice!
        public static readonly ConnectionCache ConnectionCache = new ConnectionCache();
        public static GeneralSettings GeneralSettings = new GeneralSettings(10);

        private static bool IsInitialStartupComplete;

        public MainWindowViewModel()
        {            
            StartupCommand = new Command(o => RunStartup());
            ShutDownCommand = new Command(o => RunShutdown());
            Tabs = new ObservableCollection<TabViewModel>();
            NewItemFactory = () => new TabViewModel();
        }

        public ObservableCollection<TabViewModel> Tabs { get; }

        public ICommand StartupCommand { get; }

        public ICommand ShutDownCommand { get; }

        public Func<object> NewItemFactory { get; }

        private void RunStartup()
        {
            if (IsInitialStartupComplete) return;

            string rawData;
            if (new Persistance().TryLoadRaw(out rawData))
            {
                try
                {
                    var settingsContainer = Serializer.Objectify(rawData);
                    //_connection = settingsContainer.ConnectionCache;
                    GeneralSettings = settingsContainer.GeneralSettings;
                }
                catch
                {
                    //TODO summit...                    
                }                
            }

            var tabViewModel = new TabViewModel();
            Tabs.Add(tabViewModel);
            TabablzControl.SelectItem(tabViewModel);
            tabViewModel.EditConnectionCommand.Execute(null);

            IsInitialStartupComplete = true;
        }

        private void RunShutdown()
        {
            var windowCollection = Application.Current.Windows.OfType<MainWindow>();
            if (windowCollection.Count() == 1)
                RunApplicationShutdown();
        }

        private void RunApplicationShutdown()
        {
            var layoutStructure = LayoutAnalayzer.GetLayoutStructure();
            var data = Serializer.Stringify(ConnectionCache, GeneralSettings, layoutStructure);
        }
    }    
}
