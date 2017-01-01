using System;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
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

        private const string ConnectionIdBackingStorePropertyName = "connectionId";

        internal static readonly Guid MyFeatureId = new Guid("A874603C-BEFF-4442-AF02-BA106C61B181");

        public QueryDeveloperFeatureFactory(IConnectionCache connectionCache,
            IHighlightingDefinition sqlHighlightingDefinition, IQueryFileService queryFileService, ISnackbarMessageQueue snackbarMessageQueue)
        {
            if (connectionCache == null) throw new ArgumentNullException(nameof(connectionCache));
            if (sqlHighlightingDefinition == null) throw new ArgumentNullException(nameof(sqlHighlightingDefinition));
            if (queryFileService == null) throw new ArgumentNullException(nameof(queryFileService));
            if (snackbarMessageQueue == null) throw new ArgumentNullException(nameof(snackbarMessageQueue));

            FeatureId = MyFeatureId;            

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
            var connectionPropVal = tabItem.ReadProperty(ConnectionIdBackingStorePropertyName);
            Guid connectionId;
            Connection connection = null;
            if (connectionPropVal != null && Guid.TryParse(connectionPropVal, out connectionId))
            {
                connection = _connectionCache.Get(connectionId).ValueOrDefault();
            }

            var tabViewModel = new TabViewModel(tabItem.Id, connection, _connectionCache, _sqlHighlightingDefinition, _snackbarMessageQueue);
            PopulateDocument(tabViewModel);
            var disposable = Watch(tabViewModel);

            return new TabContentLifetimeHost(tabViewModel, closeReason => Cleanup(closeReason, tabViewModel, disposable));
        }

        public void WriteToBackingStore(object tabContentViewModel, JToken into)
        {
            var tabViewModel = tabContentViewModel as TabViewModel;
            if (tabViewModel == null)
                throw new InvalidOperationException("Expected view model type of " + typeof(TabViewModel).FullName);

            into[ConnectionIdBackingStorePropertyName] = tabViewModel.ConnectionId;
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

        private void PopulateDocument(TabViewModel tabViewModel)
        {
            Task.Factory.StartNew(() =>
            {
                var fileName = _queryFileService.GetFileName(tabViewModel.Id);
                return File.Exists(fileName) ? File.ReadAllText(fileName) : string.Empty;
            }).ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    //TODO uhm something
                }
                else
                {
                    if (string.IsNullOrEmpty(tabViewModel.QueryRunnerViewModel.Document.Text))
                    {
                        tabViewModel.QueryRunnerViewModel.Document.Text = t.Result;
                    }
                }

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}