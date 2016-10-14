using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Doobry.Infrastructure;
using Doobry.Settings;
using DynamicData.Kernel;
using ICSharpCode.AvalonEdit.Highlighting;
using MaterialDesignThemes.Wpf;

namespace Doobry
{
    public class TabViewModel : INotifyPropertyChanged
    {
        private readonly IConnectionCache _connectionCache;
        private readonly GeneralSettings _generalSettings;
        private Connection _connection;
        private int _viewIndex;
        private string _documentId;
        private string _name;        

        public TabViewModel(Guid id, IConnectionCache connectionCache, IHighlightingDefinition sqlHighlightingDefinition) : this(id, null, connectionCache, sqlHighlightingDefinition)
        { }

        public TabViewModel(Guid id, Connection connection, IConnectionCache connectionCache, IHighlightingDefinition sqlHighlightingDefinition)
        {
            if (connectionCache == null) throw new ArgumentNullException(nameof(connectionCache));

            Id = id;
            _generalSettings = new GeneralSettings(10);
            _connection = connection;
            _connectionCache = connectionCache;

            FetchDocumentCommand = new Command(o => QueryRunnerViewModel.Run($"SELECT * FROM root r WHERE r.id = '{DocumentId}'"));
            EditConnectionCommand = new Command(sender => EditConnectionAsync((DependencyObject)sender));
            EditSettingsCommand = new Command(sender => EditSettingsAsync((DependencyObject)sender));
            QueryRunnerViewModel = new QueryRunnerViewModel(id, sqlHighlightingDefinition, () => _connection, () => _generalSettings, EditDocumentHandler);
            DocumentEditorViewModel = new DocumentEditorViewModel(() => _connection);            

            SetName();
        }

        public Guid Id { get; }

        public IObservable<DocumentChangedUnit> DocumentChangedObservable => QueryRunnerViewModel.DocumentChangedObservable;

        public string Name
        {
            get { return _name; }
            private set { this.MutateVerbose(ref _name, value, RaisePropertyChanged()); }
        }

        public string DocumentId
        {
            get { return _documentId; }
            set { this.MutateVerbose(ref _documentId, value, RaisePropertyChanged()); }
        }

        public int ViewIndex
        {
            get { return _viewIndex; }
            set { this.MutateVerbose(ref _viewIndex, value, RaisePropertyChanged()); }
        }

        public ICommand FetchDocumentCommand { get; }

        public ICommand EditConnectionCommand { get; }

        public ICommand EditSettingsCommand { get; }

        public QueryRunnerViewModel QueryRunnerViewModel { get; }

        public DocumentEditorViewModel DocumentEditorViewModel { get; }

        public Guid? ConnectionId => _connection?.Id;

        private void EditDocumentHandler(Result result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            DocumentEditorViewModel.Document.Text = result.Data;
            ViewIndex = 1;
        }

        private async void EditConnectionAsync(DependencyObject sender)
        {
            Debug.Assert(sender != null);

            var connectionOption = await new ConnectionManagementController(_connectionCache).Select(sender);

            SetConnection(connectionOption.ValueOr(() => _connection));
        }

        private async void EditSettingsAsync(DependencyObject sender)
        {
            var viewModel = new GeneralSettingsEditorViewModel
            {
                MaxItemCount = _generalSettings.MaxItemCount
            };
            var settingsEditor = new GeneralSettingsEditor
            {
                DataContext = viewModel
            };

            await ShowDialogAsync(settingsEditor, "Settings", PackIconKind.Settings, sender);            

            _generalSettings.MaxItemCount = viewModel.MaxItemCount;            
        }

        private void SetConnection(Connection connection)
        {
            _connection = connection;
            SetName();
        }

        private void SetName()
        {
            Name = _connection?.Label ?? "(no connection)";
        }

        private static async Task<bool> ShowDialogAsync(object content, string title, PackIconKind icon, DependencyObject sender)
        {
            var dialogContentControl = new DialogContentControl
            {
                Content = content,
                Title = title,
                Icon = icon,
                ShowStandardButtons = false
            };
            
            var result = await (sender == null ? DialogHost.Show(dialogContentControl) : sender.ShowDialog(dialogContentControl));

            return bool.TrueString.Equals(result);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}