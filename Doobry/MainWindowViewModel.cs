using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Doobry.Infrastructure;
using Doobry.Settings;
using Dragablz;
using Dragablz.Dockablz;
using DynamicData.Kernel;
using MaterialDesignThemes.Wpf;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Doobry
{
    public class MainWindowViewModel// : INotifyPropertyChanged
    {
        private readonly IConnectionCache _connectionCache;
        public static GeneralSettings GeneralSettings = new GeneralSettings(10);

        private static bool _isStartupInitiated;

        public MainWindowViewModel(IConnectionCache connectionCache)
        {
            if (connectionCache == null) throw new ArgumentNullException(nameof(connectionCache));

            _connectionCache = connectionCache;

            StartupCommand = new Command(RunStartup);
            ShutDownCommand = new Command(o => RunShutdown());
            Tabs = new ObservableCollection<TabViewModel>();            
        }

        public ObservableCollection<TabViewModel> Tabs { get; }

        public ICommand StartupCommand { get; }

        public ICommand ShutDownCommand { get; }

        private void RunStartup(object sender)
        {
            if (_isStartupInitiated) return;
            _isStartupInitiated = true;

            //TODO assert this stuff, provide a fall back
            var senderDependencyObject = sender as DependencyObject;
            var mainWindow = Window.GetWindow(senderDependencyObject) as MainWindow;
            var rootTabControl = mainWindow.InitialTabablzControl;


            string rawData;
            if (new Persistance().TryLoadRaw(out rawData))
            {
                try
                {
                    var settingsContainer = Serializer.Objectify(rawData);
                    //_connection = settingsContainer.ConnectionCache;
                    GeneralSettings = settingsContainer.GeneralSettings;

                    RestoreLayout(rootTabControl, settingsContainer.LayoutStructure, settingsContainer.ConnectionCache);
                }
                catch (Exception exc)
                {
                    System.Diagnostics.Debug.WriteLine(exc.Message);
                    //rootTabControl.ShowDialog(exc.Message);

                    //TODO summit...                    
                }                
            }

            if (!TabablzControl.GetLoadedInstances().SelectMany(tc => tc.Items.OfType<object>()).Any())
            {
                var tabViewModel = new TabViewModel(_connectionCache);
                Tabs.Add(tabViewModel);
                TabablzControl.SelectItem(tabViewModel);
                tabViewModel.EditConnectionCommand.Execute(null);
            }            
        }

        private static void RestoreLayout(TabablzControl rootTabControl, LayoutStructure layoutStructure, IConnectionCache connectionCache)
        {
            //we only currently support a single window, can build on in future
            var layoutStructureWindow = layoutStructure.Windows.Single();
            if (layoutStructureWindow.Branches.Any())
            {
                var branchIndex = layoutStructureWindow.Branches.ToDictionary(b => b.Id);
                var rootBranch = GetRoot(branchIndex);
                var tabSetIndex = layoutStructureWindow.TabSets.ToDictionary(ts => ts.Id);
                BuildLayout(rootTabControl, rootBranch, branchIndex, tabSetIndex, connectionCache);
            }
            else
            {
                var layoutStructureTabSet = layoutStructureWindow.TabSets.Single();
                BuildTabSet(layoutStructureTabSet, rootTabControl, connectionCache);
            }
        }

        private static void BuildLayout(
            TabablzControl intoTabablzControl, 
            LayoutStructureBranch layoutStructureBranch,
            IDictionary<Guid, LayoutStructureBranch> layoutStructureBranchIndex,
            IDictionary<Guid, LayoutStructureTabSet> layoutStructureTabSetIndex,
            IConnectionCache connectionCache)
        {
            var newSiblingTabablzControl = new TabablzControl
            {
                HeaderMemberPath = "Name",
                BorderThickness = new Thickness(0),
                ShowDefaultAddButton = true,
                ShowDefaultCloseButton = true,
                //TODO we can inject this now we have a proper CI container
                NewItemFactory = App.NewItemFactory
            };
            var branchResult = Layout.Branch(intoTabablzControl, newSiblingTabablzControl, layoutStructureBranch.Orientation, false, layoutStructureBranch.Ratio);

            if (layoutStructureBranch.ChildFirstBranchId.HasValue)
            {
                var firstChildBranch = layoutStructureBranchIndex[layoutStructureBranch.ChildFirstBranchId.Value];
                BuildLayout(branchResult.TabablzControl, firstChildBranch, layoutStructureBranchIndex, layoutStructureTabSetIndex, connectionCache);
            }
            else if (layoutStructureBranch.ChildFirstTabSetId.HasValue)
            {
                BuildTabSet(layoutStructureTabSetIndex[layoutStructureBranch.ChildFirstTabSetId.Value], intoTabablzControl,
                    connectionCache);
            }            

            if (layoutStructureBranch.ChildSecondBranchId.HasValue)
            {
                var secondChildBranch = layoutStructureBranchIndex[layoutStructureBranch.ChildSecondBranchId.Value];
                BuildLayout(branchResult.TabablzControl, secondChildBranch, layoutStructureBranchIndex, layoutStructureTabSetIndex, connectionCache);
            }
            else if (layoutStructureBranch.ChildSecondTabSetId.HasValue)
            {
                BuildTabSet(layoutStructureTabSetIndex[layoutStructureBranch.ChildSecondTabSetId.Value], newSiblingTabablzControl,
                    connectionCache);
            }
        }

        private static void BuildTabSet(LayoutStructureTabSet layoutStructureTabSet, TabablzControl intoTabablzControl, IConnectionCache connectionCache)
        {
            foreach (var layoutStructureTabItem in layoutStructureTabSet.TabItems)
            {                
                Connection connection = null;
                if (layoutStructureTabItem.ConnectionId.HasValue)
                {
                    connection = connectionCache.Get(layoutStructureTabItem.ConnectionId.Value).ValueOrDefault();
                }                
                var tabViewModel = new TabViewModel(connection, connectionCache);
                intoTabablzControl.AddToSource(tabViewModel);                
            }
        }

        private static LayoutStructureBranch GetRoot(Dictionary<Guid, LayoutStructureBranch> branches)
        {
            var lookup = branches.Values.SelectMany(ChildBranchIds).Distinct().ToLookup(guid => guid);
            return branches.Values.Single(branch => !lookup.Contains(branch.Id));
        }

        private static IEnumerable<Guid> ChildBranchIds(LayoutStructureBranch branch)
        {
            if (branch.ChildFirstBranchId.HasValue)
                yield return branch.ChildFirstBranchId.Value;
            if (branch.ChildSecondBranchId.HasValue)
                yield return branch.ChildSecondBranchId.Value;
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
            var data = Serializer.Stringify(_connectionCache, GeneralSettings, layoutStructure);

            new Persistance().TrySaveRaw(data);

            Application.Current.Shutdown();
        }
    }    
}
