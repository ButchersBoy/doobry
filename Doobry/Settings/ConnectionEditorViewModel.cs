using System;
using System.ComponentModel;
using System.Windows.Input;
using Doobry.Infrastructure;

namespace Doobry.Settings
{
    public enum ConnectionEditorDisplayMode
    {
        Dialog,
        MultiEdit
    }

    public class ConnectionEditorViewModel : INotifyPropertyChanged
    {
        private string _label;
        private string _host;
        private string _authorisationKey;
        private string _databaseId;
        private string _collectionId;
        private ConnectionEditorDisplayMode _displayMode;

        public ConnectionEditorViewModel(Action<ConnectionEditorViewModel> saveHandler, Action cancelHandler)
            : this(null, saveHandler, cancelHandler)
            
        { }

        public ConnectionEditorViewModel(Connection connection, Action<ConnectionEditorViewModel> saveHandler, Action cancelHandler)
        {
            if (saveHandler == null) throw new ArgumentNullException(nameof(saveHandler));
            if (cancelHandler == null) throw new ArgumentNullException(nameof(cancelHandler));

            SaveCommand = new Command(_ => saveHandler(this));
            CancelCommand = new Command(_ => cancelHandler());
            ExploreToSettingsFileCommand = new Command(_ => ExploreToSettingsFile());

            if (connection == null) return;

            Id = connection.Id;
            _label = connection.Label;
            _host = connection.Host;
            _authorisationKey = connection.AuthorisationKey;
            _databaseId = connection.DatabaseId;
            _collectionId = connection.CollectionId;
        }

        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }

        public ICommand ExploreToSettingsFileCommand { get; }

        public Guid? Id { get; }

        public string SettingsConfigurationFilePath => Persistance.ConfigurationFilePath;

        public string Label
        {
            get { return _label; }
            set { this.MutateVerbose(ref _label, value, RaisePropertyChanged()); }
        }

        public string Host
        {
            get { return _host; }
            set { this.MutateVerbose(ref _host, value, RaisePropertyChanged()); }
        }

        public string AuthorisationKey
        {
            get { return _authorisationKey; }
            set { this.MutateVerbose(ref _authorisationKey, value, RaisePropertyChanged()); }
        }

        public string DatabaseId
        {
            get { return _databaseId; }
            set { this.MutateVerbose(ref _databaseId, value, RaisePropertyChanged()); }
        }

        public string CollectionId
        {
            get { return _collectionId; }
            set { this.MutateVerbose(ref _collectionId, value, RaisePropertyChanged()); }
        }

        public ConnectionEditorDisplayMode DisplayMode
        {
            get { return _displayMode; }
            set { this.MutateVerbose(ref _displayMode, value, RaisePropertyChanged()); }
        }

        private void ExploreToSettingsFile()
        {
            System.Diagnostics.Process.Start("Explorer", $"/select,\"{SettingsConfigurationFilePath}\"");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}
