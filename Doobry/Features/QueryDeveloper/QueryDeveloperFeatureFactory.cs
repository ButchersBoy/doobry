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
using StructureMap;
using StructureMap.Pipeline;

namespace Doobry.Features.QueryDeveloper
{
    public class QueryDeveloperFeatureFactory : IFeatureFactory
    {
        private readonly IContainer _container;
        private const string ConnectionIdBackingStorePropertyName = "connectionId";

        internal static readonly Guid MyFeatureId = new Guid("A874603C-BEFF-4442-AF02-BA106C61B181");
        private readonly IQueryFileService _queryFileService;

        public QueryDeveloperFeatureFactory(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            _container = container;
            FeatureId = MyFeatureId;

            _queryFileService = _container.GetInstance<IQueryFileService>();
        }

        public Guid FeatureId { get; }

        public ITabContentLifetimeHost CreateTabContent()
        {
            var explicitArguments = new ExplicitArguments();
            explicitArguments.Set(typeof(Guid), Guid.NewGuid());
            explicitArguments.Set(typeof(ExplicitConnection), null);

            var tabViewModel = _container.GetInstance<QueryDeveloperViewModel>(explicitArguments);
            var disposable = Watch(tabViewModel);
            return new TabContentLifetimeHost(tabViewModel, closeReason => Cleanup(closeReason, tabViewModel, disposable));
        }

        public ITabContentLifetimeHost RestoreTabContent(LayoutStructureTabItem tabItem)
        {
            var connectionPropVal = tabItem.ReadProperty(ConnectionIdBackingStorePropertyName);
            Guid connectionId;
            ExplicitConnection explicitConnection = null;
            if (connectionPropVal != null && Guid.TryParse(connectionPropVal, out connectionId))
            {
                explicitConnection = _container.GetInstance<IExplicitConnectionCache>().Get(connectionId).ValueOrDefault();
            }

            var explicitArguments = new ExplicitArguments();
            explicitArguments.Set(typeof(Guid), Guid.NewGuid());
            explicitArguments.Set(typeof(ExplicitConnection), explicitConnection);

            var tabViewModel = _container.GetInstance<QueryDeveloperViewModel>(explicitArguments);
            PopulateDocument(tabViewModel);
            var disposable = Watch(tabViewModel);

            return new TabContentLifetimeHost(tabViewModel, closeReason => Cleanup(closeReason, tabViewModel, disposable));
        }

        public void WriteToBackingStore(object tabContentViewModel, JToken into)
        {
            var tabViewModel = tabContentViewModel as QueryDeveloperViewModel;
            if (tabViewModel == null)
                throw new InvalidOperationException("Expected view model type of " + typeof(QueryDeveloperViewModel).FullName);

            into[ConnectionIdBackingStorePropertyName] = tabViewModel.ConnectionId;
        }

        private void Cleanup(TabCloseReason tabCloseReason, QueryDeveloperViewModel queryDeveloperViewModel, IDisposable watchSubscription)
        {
            watchSubscription.Dispose();
            if (tabCloseReason == TabCloseReason.TabClosed)
                RemoveDocument(queryDeveloperViewModel);
        }

        private IDisposable Watch(QueryDeveloperViewModel queryDeveloperViewModel)
        {
            return queryDeveloperViewModel.DocumentChangedObservable.Throttle(TimeSpan.FromSeconds(3))
                .ObserveOn(new EventLoopScheduler())
                .Subscribe(SaveDocument);         
        }

        private void RemoveDocument(QueryDeveloperViewModel queryDeveloperViewModel)
        {
            var fileName = _queryFileService.GetFileName(queryDeveloperViewModel.Id);
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

        private void PopulateDocument(QueryDeveloperViewModel queryDeveloperViewModel)
        {
            Task.Factory.StartNew(() =>
            {
                var fileName = _queryFileService.GetFileName(queryDeveloperViewModel.Id);
                return File.Exists(fileName) ? File.ReadAllText(fileName) : string.Empty;
            }).ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    //TODO uhm something
                }
                else
                {
                    if (string.IsNullOrEmpty(queryDeveloperViewModel.QueryRunnerViewModel.Document.Text))
                    {
                        queryDeveloperViewModel.QueryRunnerViewModel.Document.Text = t.Result;
                    }
                }

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}