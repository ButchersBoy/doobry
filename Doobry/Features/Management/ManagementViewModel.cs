using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Doobry.DocumentDb;
using Doobry.Infrastructure;
using Doobry.Settings;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Kernel;
using MaterialDesignThemes.Wpf;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Doobry.Features.Management
{
    public enum ManagementActionStep
    {
        CollectInput,
        Run,
        ReportFailure
    }

    public class ManagementActionViewModel<TProperties> : INotifyPropertyChanged, IDisposable
        where TProperties : INotifyDataErrorInfo, INotifyPropertyChanged
    {
        private readonly Action<TProperties> _actionRunner;
        private readonly Action _onEnd;
        private readonly IDisposable _disposable;
        private ManagementActionStep _step;
        private string _error;

        public ManagementActionViewModel(TProperties propertiesViewModel, Action<TProperties> actionRunner, Action onEnd)
        {
            _actionRunner = actionRunner;
            _onEnd = onEnd;
            PropertiesViewModel = propertiesViewModel;
            
            var okCommand = new Command(_ => MangeRun(), _ => !propertiesViewModel.HasErrors);
            _disposable = propertiesViewModel.WhenPropertyChanged(p => p.HasErrors).Subscribe(v => okCommand.Refresh());

            OkCommand = okCommand;
            CancelCommand = new Command(_ => onEnd());
            DismissErrorCommand = new Command(_ => Step = ManagementActionStep.CollectInput);
        }

        public ManagementActionStep Step
        {
            get { return _step; }
            private set { this.MutateVerbose(ref _step, value, RaisePropertyChanged()); }
        }

        public ICommand OkCommand { get; }

        public ICommand CancelCommand { get; }

        public ICommand DismissErrorCommand { get; }

        public TProperties PropertiesViewModel { get; }

        private void MangeRun()
        {
            Step = ManagementActionStep.Run;            
            Task.Factory.StartNew(() => _actionRunner(PropertiesViewModel))
                .ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        Step = ManagementActionStep.ReportFailure;
                        Error = t.Exception.ToString();
                    }
                    else
                        _onEnd();
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public string Error
        {
            get { return _error; }
            private set { this.MutateVerbose(ref _error, value, RaisePropertyChanged()); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }

    public class CreateDatabaseProperties : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private string _databaseId;
        private bool _hasErrors;
        private string _error;

        public CreateDatabaseProperties()
        {
            DatabaseId = string.Empty;
        }

        public string DatabaseId
        {
            get { return _databaseId; }
            set
            {
                this.MutateVerbose(ref _databaseId, value, RaisePropertyChanged());
                Error = string.IsNullOrWhiteSpace(DatabaseId) ? "Required" : null;
                HasErrors = Error != null;
            }
        }

        public bool HasErrors
        {
            get { return _hasErrors; }
            private set { this.MutateVerbose(ref _hasErrors, value, RaisePropertyChanged()); }
        }

        public string Error
        {
            get { return _error; }
            private set { this.MutateVerbose(ref _error, value, RaisePropertyChanged()); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }        

        public IEnumerable GetErrors(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(DatabaseId):
                    if (string.IsNullOrWhiteSpace(DatabaseId))
                        yield return "Required";
                    break;
            }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected virtual void OnErrorsChanged(DataErrorsChangedEventArgs e)
        {
            ErrorsChanged?.Invoke(this, e);
        }
    }

    public interface IManagementActionsController
    {
        void AddDatabase(HostNode host);
    }

    public class ManagementActionsController : IManagementActionsController
    {
        private readonly IDialogTargetFinder _dialogTargetFinder;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;

        public ManagementActionsController(IDialogTargetFinder dialogTargetFinder, ISnackbarMessageQueue snackbarMessageQueue)
        {
            if (dialogTargetFinder == null) throw new ArgumentNullException(nameof(dialogTargetFinder));
            if (snackbarMessageQueue == null) throw new ArgumentNullException(nameof(snackbarMessageQueue));

            _dialogTargetFinder = dialogTargetFinder;
            _snackbarMessageQueue = snackbarMessageQueue;
        }

        public async void AddDatabase(HostNode host)
        {
            var view = new ManagementAction();            
            var createDatabaseProperties = new CreateDatabaseProperties();

            ManagementActionViewModel<CreateDatabaseProperties> model = null;
            await DialogHost.Show(view, _dialogTargetFinder.SuggestDialogHostIdentifier(),
                delegate(object sender, DialogOpenedEventArgs args)
                {
                    model = new ManagementActionViewModel<CreateDatabaseProperties>(createDatabaseProperties, p =>
                        DoAddDatabase(p, host), () => args.Session.Close());
                    view.DataContext = model;
                });
            model?.Dispose();
        }

        private static async void DoAddDatabase(CreateDatabaseProperties properties, HostNode hostNode)
        {
            var database = new Database
            {
                Id = properties.DatabaseId
            };
            await CreateDocumentClient(hostNode).CreateDatabaseAsync(database);
        }

        private static DocumentClient CreateDocumentClient(HostNode hostNode)
        {
            return new DocumentClient(new Uri(hostNode.Host), hostNode.AuthorisationKey);
        }
    }

    public class ManagementViewModel : INamed, IDisposable
    {
        private readonly IDisposable _disposable;

        public ManagementViewModel(
            IExplicitConnectionCache explicitConnectionCache, 
            IImplicitConnectionCache implicitConnectionCache,
            IManagementActionsController managementActionsController,
            DispatcherScheduler dispatcherScheduler)
        {
            if (explicitConnectionCache == null) throw new ArgumentNullException(nameof(explicitConnectionCache));
            if (implicitConnectionCache == null) throw new ArgumentNullException(nameof(implicitConnectionCache));
            if (managementActionsController == null)
                throw new ArgumentNullException(nameof(managementActionsController));
            if (dispatcherScheduler == null) throw new ArgumentNullException(nameof(dispatcherScheduler));

            Name = "DB Manager";

            ReadOnlyObservableCollection<HostNode> nodes;
            _disposable = explicitConnectionCache.Connect()
                //user could dupe the connection
                .Group(explicitCn => (Connection) explicitCn)
                .FullJoin(implicitConnectionCache.Connect().Group(implicitCn => (Connection) implicitCn),
                    implicitGroup => implicitGroup.Key,
                    (cn, left, right) =>
                        new GroupedConnection(GetOptionalConnections(left), GetOptionalConnections(right)))
                .Transform(groupedConnection => new HostNode(groupedConnection, managementActionsController))
                .DisposeMany()
                .ObserveOn(dispatcherScheduler)
                .Bind(out nodes)
                .Subscribe();
            Hosts = nodes;
        }

        private static IEnumerable<TConnection> GetOptionalConnections<TConnection, TKey>(Optional<IGroup<TConnection, TKey, Connection>> left) where TConnection : Connection
        {
            return left.HasValue
                ? left.Value.Cache.Items
                : Enumerable.Empty<TConnection>();
        }

        public string Name { get; }

        public IEnumerable<HostNode> Hosts { get; }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }

    public class HostNode : IDisposable
    {
        private readonly SourceList<DatabaseNode> _sourceList = new SourceList<DatabaseNode>();
        private readonly IDisposable _disposable;

        public HostNode(GroupedConnection groupedConnection, IManagementActionsController managementActionsController)
        {
            if (groupedConnection == null) throw new ArgumentNullException(nameof(groupedConnection));
            if (managementActionsController == null)
                throw new ArgumentNullException(nameof(managementActionsController));

            Host = groupedConnection.Host;
            AuthorisationKey = groupedConnection.AuthorisationKey;
            var authKeyHint = groupedConnection.AuthorisationKey ?? "";
            AuthorisationKeyHint =
                (authKeyHint.Length > 0 ? authKeyHint.Substring(0, Math.Min(authKeyHint.Length, 5)) : "")
                + "...";

            CreateDatabaseCommand = new Command(_ => managementActionsController.AddDatabase(this));

            ReadOnlyObservableCollection<DatabaseNode> nodes;
            _disposable = _sourceList.Connect().Bind(out nodes).Subscribe();
            Databases = nodes;

            if (string.IsNullOrEmpty(groupedConnection.DatabaseId)) return;

            _sourceList.Add(string.IsNullOrEmpty(groupedConnection.CollectionId)
                ? new DatabaseNode(this, groupedConnection.DatabaseId)
                : new DatabaseNode(this, groupedConnection.DatabaseId, groupedConnection.CollectionId));
        }

        public ICommand CreateDatabaseCommand { get; }

        public string Host { get; }

        public string AuthorisationKeyHint { get; }

        public string AuthorisationKey { get; }

        public IEnumerable<DatabaseNode> Databases { get; }

        private void CreateDatabase()
        {

        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }

    public class DatabaseNode
    {
        public DatabaseNode(HostNode owner, string databaseId, params string[] collectionIds)
        {
            Owner = owner;
            DatabaseId = databaseId;
            Collections = collectionIds.Select(c => new CollectionNode(this, c));

            CreateCollectionCommand = new Command(_ => CreateCollection());
            DeleteDatabaseCommand = new Command(_ => DeleteDatabase());
        }

        public ICommand DeleteDatabaseCommand { get; }

        public ICommand CreateCollectionCommand { get; }

        public HostNode Owner { get; }

        public string DatabaseId { get; }

        public IEnumerable<CollectionNode> Collections { get; }

        private void DeleteDatabase()
        {

        }

        private void CreateCollection()
        {

        }
    }

    public class CollectionNode
    {
        public CollectionNode(DatabaseNode owner, string collectionId)
        {
            Owner = owner;
            CollectionId = collectionId;

            DeleteCollectionCommand = new Command(_ => DeleteCollection());
        }

        public ICommand DeleteCollectionCommand { get; }

        public DatabaseNode Owner { get; }

        public string CollectionId { get; }

        private void DeleteCollection()
        {

        }
    }
}
