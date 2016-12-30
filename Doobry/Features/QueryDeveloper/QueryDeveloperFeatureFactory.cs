using System;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Doobry.Settings;
using DynamicData.Kernel;
using ICSharpCode.AvalonEdit.Highlighting;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json.Linq;

namespace Doobry.Features.QueryDeveloper
{
    public class QueryDeveloperFeatureFactory : IFeatureFactory
    {
        private readonly IConnectionCache _connectionCache;
        private readonly IHighlightingDefinition _sqlHighlightingDefinition;
        private readonly IQueryFileService _queryFileService;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;        

        public QueryDeveloperFeatureFactory(IConnectionCache connectionCache,
            IHighlightingDefinition sqlHighlightingDefinition, IQueryFileService queryFileService, ISnackbarMessageQueue snackbarMessageQueue)
        {
            if (connectionCache == null) throw new ArgumentNullException(nameof(connectionCache));
            if (sqlHighlightingDefinition == null) throw new ArgumentNullException(nameof(sqlHighlightingDefinition));
            if (queryFileService == null) throw new ArgumentNullException(nameof(queryFileService));
            if (snackbarMessageQueue == null) throw new ArgumentNullException(nameof(snackbarMessageQueue));

            FeatureId = new Guid("A874603C-BEFF-4442-AF02-BA106C61B181");            

            _connectionCache = connectionCache;
            _sqlHighlightingDefinition = sqlHighlightingDefinition;
            _queryFileService = queryFileService;
            _snackbarMessageQueue = snackbarMessageQueue;
        }

        public Guid FeatureId { get; }

        public ITabContentLifetimeHost CreateTabContent()
        {
            var tabViewModel = new TabViewModel(Guid.NewGuid(), _connectionCache, _sqlHighlightingDefinition, _snackbarMessageQueue);
            var disposable = Watch(tabViewModel);
            return new TabContentLifetimeHost(tabViewModel, closeReason => Cleanup(closeReason, tabViewModel, disposable));
        }

        public ITabContentLifetimeHost RestoreTabContent(LayoutStructureTabItem tabItem)
        {
            var connectionId = tabItem.ReadProperty<Guid?>("ConnectionId");

            Connection connection = null;
            if (connectionId.HasValue)
            {
                connection = _connectionCache.Get(connectionId.Value).ValueOrDefault();
            }

            var tabViewModel = new TabViewModel(tabItem.Id, connection, _connectionCache, _sqlHighlightingDefinition, _snackbarMessageQueue);
            var disposable = Watch(tabViewModel);

            return new TabContentLifetimeHost(tabViewModel, closeReason => Cleanup(closeReason, tabViewModel, disposable));
        }

        public void WriteToBackingStore(object tabContentViewModel, JToken into)
        {
            var tabViewModel = tabContentViewModel as TabViewModel;
            if (tabViewModel == null)
                throw new InvalidOperationException("Expected view model type of " + typeof(TabViewModel).FullName);

            into["ConnectionId"] = tabViewModel.ConnectionId;
        }

        private void Cleanup(TabCloseReason tabCloseReason, TabViewModel tabViewModel, IDisposable watchSubscription)
        {
            watchSubscription.Dispose();
            if (tabCloseReason == TabCloseReason.TabClosed)
                RemoveDocument(tabViewModel);
        }

        private IDisposable Watch(TabViewModel tabViewModel)
        {
            return tabViewModel.DocumentChangedObservable.Throttle(TimeSpan.FromSeconds(3))
                .ObserveOn(new EventLoopScheduler())
                .Subscribe(SaveDocument);         
        }

        private void RemoveDocument(TabViewModel tabViewModel)
        {
            var fileName = _queryFileService.GetFileName(tabViewModel.Id);
            if (!File.Exists(fileName)) return;

            try
            {
                File.Delete(fileName);
            }
            catch { /* TODO uhm something */ }
        }

        private void SaveDocument(DocumentChangedUnit documentChangedUnit)
        {
            try
            {
                var directoryName = Path.GetDirectoryName(_queryFileService.GetFileName(documentChangedUnit.TabId));
                Directory.CreateDirectory(directoryName);
                File.WriteAllText(_queryFileService.GetFileName(documentChangedUnit.TabId), documentChangedUnit.Text);
            }
            catch (Exception)
            {
                //TODO...erm summit                
            }
        }        
    }
}