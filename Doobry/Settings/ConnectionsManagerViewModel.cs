using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Doobry.Infrastructure;
using DynamicData;

namespace Doobry.Settings
{    
    public class ConnectionsManagerViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly IConnectionCache _connectionCache;
        private readonly ReadOnlyObservableCollection<Connection> _connections;
        private readonly IDisposable _disposable;
        private Connection _selectedConnection;
        private ConnectionEditorViewModel _connectionEditorEditorViewModel;
        private bool _shouldShowSelector;
        private ConnectionsManagerMode _mode;

        public ConnectionsManagerViewModel(IConnectionCache connectionCache)
        {
            _connectionCache = connectionCache;
            if (connectionCache == null) throw new ArgumentNullException(nameof(connectionCache));

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
                var connection = o as Connection;
                if (connection == null) return;
                ConnectionEditor = new ConnectionEditorViewModel(connection, SaveConnection, () => Mode = ConnectionsManagerMode.Selector)
                {
                    DisplayMode = ConnectionEditorDisplayMode.MultiEdit
                };
                Mode = ConnectionsManagerMode.ItemEditor;
            }, o => o is Connection);

            _disposable = connectionCache.Connect().Bind(out _connections).Subscribe();
            if (_connections.Count == 0) AddConnectionCommand.Execute(null);
        }

        public ConnectionsManagerMode Mode
        {
            get { return _mode; }
            set { this.MutateVerbose(ref _mode, value, RaisePropertyChanged()); }
        }

        public ICommand AddConnectionCommand { get; }

        public ICommand EditConnectionCommand { get; }

        public Connection SelectedConnection
        {
            get { return _selectedConnection; }
            set { this.MutateVerbose(ref _selectedConnection, value, RaisePropertyChanged()); }
        }

        public ConnectionEditorViewModel ConnectionEditor
        {
            get { return _connectionEditorEditorViewModel; }
            private set { this.MutateVerbose(ref _connectionEditorEditorViewModel, value, RaisePropertyChanged()); }
        }

        public ReadOnlyObservableCollection<Connection> Connections => _connections;

        public bool ShouldShowSelector
        {
            get { return _shouldShowSelector; }
            private set { this.MutateVerbose(ref _shouldShowSelector, value, RaisePropertyChanged()); }
        }

        private void SaveConnection(ConnectionEditorViewModel viewModel)
        {
            var connection = new Connection(viewModel.Id.GetValueOrDefault(Guid.NewGuid()), viewModel.Label, viewModel.Host,
                viewModel.AuthorisationKey, viewModel.DatabaseId, viewModel.CollectionId);
            _connectionCache.AddOrUpdate(connection);            
            Mode = ConnectionsManagerMode.Selector;
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
}