using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Doobry.Infrastructure;
using DynamicData;
using DynamicData.Controllers;
using DynamicData.Kernel;
using DynamicData.Operators;
using MaterialDesignThemes.Wpf;

namespace Doobry.Settings
{    
    public class ConnectionsManagerViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly IExplicitConnectionCache _explicitConnectionCache;
        private readonly ReadOnlyObservableCollection<ExplicitConnection> _connections;
        private readonly IDisposable _connectionCacheSubscription;
        private readonly SnackbarMessageQueue _snackbarMessageQueue = new SnackbarMessageQueue();
        private ExplicitConnection _selectedExplicitConnection;
        private ConnectionEditorViewModel _connectionEditorEditorViewModel;
        private bool _shouldShowSelector;
        private ConnectionsManagerMode _mode;

        public ConnectionsManagerViewModel(IExplicitConnectionCache explicitConnectionCache)
        {
            if (explicitConnectionCache == null) throw new ArgumentNullException(nameof(explicitConnectionCache));

            _explicitConnectionCache = explicitConnectionCache;

            AddConnectionCommand = new Command(_ =>
            {
                var vm = new ConnectionEditorViewModel(SaveConnection, () => Mode = ConnectionsManagerMode.Selector)
                {
                    DisplayMode = ConnectionEditorDisplayMode.MultiEdit
                };
                ConnectionEditor = vm;
                Mode = ConnectionsManagerMode.ItemEditor;
            });
            EditConnectionCommand = new Command(o =>
            {
                var connection = o as ExplicitConnection;
                if (connection == null) return;
                ConnectionEditor = new ConnectionEditorViewModel(connection, SaveConnection, () => Mode = ConnectionsManagerMode.Selector)
                {
                    DisplayMode = ConnectionEditorDisplayMode.MultiEdit
                };
                Mode = ConnectionsManagerMode.ItemEditor;
            }, o => o is ExplicitConnection);            
            DeleteConnectionCommand = new Command(DeleteConnection, o => o is ExplicitConnection);

            _connectionCacheSubscription =
                explicitConnectionCache.Connect()
                    .Sort(SortExpressionComparer<ExplicitConnection>.Ascending(c => c.Label))
                    .Bind(out _connections)
                    .Subscribe();

            if (_connections.Count == 0) AddConnectionCommand.Execute(null);
        }

        private void DeleteConnection(object o)
        {
            var connection = o as ExplicitConnection;
            if (connection == null) return;

            var optional = _explicitConnectionCache.Get(connection.Id);
            if (!optional.HasValue)
                return;

            _explicitConnectionCache.Delete(connection.Id);
            SnackbarMessageQueue.Enqueue($"Deleted {connection.Label}.", "UNDO", _explicitConnectionCache.AddOrUpdate,
                optional.Value, true);
        }

        public ConnectionsManagerMode Mode
        {
            get { return _mode; }
            set { this.MutateVerbose(ref _mode, value, RaisePropertyChanged()); }
        }

        public ICommand AddConnectionCommand { get; }

        public ICommand EditConnectionCommand { get; }

        public ICommand DeleteConnectionCommand { get; }

        public ExplicitConnection SelectedExplicitConnection
        {
            get { return _selectedExplicitConnection; }
            set { this.MutateVerbose(ref _selectedExplicitConnection, value, RaisePropertyChanged()); }
        }

        public ConnectionEditorViewModel ConnectionEditor
        {
            get { return _connectionEditorEditorViewModel; }
            private set { this.MutateVerbose(ref _connectionEditorEditorViewModel, value, RaisePropertyChanged()); }
        }

        public ReadOnlyObservableCollection<ExplicitConnection> Connections => _connections;

        public bool ShouldShowSelector
        {
            get { return _shouldShowSelector; }
            private set { this.MutateVerbose(ref _shouldShowSelector, value, RaisePropertyChanged()); }
        }

        public ISnackbarMessageQueue SnackbarMessageQueue => _snackbarMessageQueue;

        private void SaveConnection(ConnectionEditorViewModel viewModel)
        {
            var connection = new ExplicitConnection(viewModel.Id.GetValueOrDefault(Guid.NewGuid()), viewModel.Label, viewModel.Host,
                viewModel.AuthorisationKey, viewModel.DatabaseId, viewModel.CollectionId);
            _explicitConnectionCache.AddOrUpdate(connection);            
            Mode = ConnectionsManagerMode.Selector;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }

        public void Dispose()
        {
            _connectionCacheSubscription.Dispose();
            _snackbarMessageQueue.Dispose();
        }
    }
}