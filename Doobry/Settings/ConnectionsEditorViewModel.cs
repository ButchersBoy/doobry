using Doobry.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Doobry.Settings
{
    public class ConnectionsEditorViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<ConnectionEditorViewModel> _connections = new ObservableCollection<ConnectionEditorViewModel>();
        private ConnectionEditorViewModel _selectedConnection;

        public ConnectionsEditorViewModel(IEnumerable<ConnectionEditorViewModel> connectionEditorViewModels)
        {
            if (connectionEditorViewModels == null) throw new ArgumentNullException(nameof(connectionEditorViewModels));

            foreach (var connectionEditorViewModel in connectionEditorViewModels)
            {
                _connections.Add(connectionEditorViewModel);
            }            

            AddConnectionCommand = new Command(_ =>
            {
                var vm = new ConnectionEditorViewModel();
                _connections.Add(vm);
                SelectedConnection = vm;
            });
        }

        public ICommand AddConnectionCommand { get; }

        public ConnectionEditorViewModel SelectedConnection
        {
            get { return _selectedConnection; }
            set { this.MutateVerbose(ref _selectedConnection, value, RaisePropertyChanged()); }
        }

        public ReadOnlyObservableCollection<ConnectionEditorViewModel> Connections => new ReadOnlyObservableCollection<ConnectionEditorViewModel>(_connections);

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}
