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

namespace Doobry.Features.QueryDeveloper
{
    public class QueryDeveloperViewModel : INamed, INotifyPropertyChanged
    {
        private readonly IExplicitConnectionCache _explicitConnectionCache;
        private readonly GeneralSettings _generalSettings;
        private ExplicitConnection _explicitConnection;
        private int _viewIndex;
        private string _documentId;
        private string _name;        

        public QueryDeveloperViewModel(Guid fileId, IExplicitConnectionCache explicitConnectionCache, IHighlightingDefinition sqlHighlightingDefinition, ISnackbarMessageQueue snackbarMessageQueue, IDialogTargetFinder dialogTargetFinder) 
            : this(fileId, null, explicitConnectionCache, sqlHighlightingDefinition, snackbarMessageQueue, dialogTargetFinder)
        { }

        public QueryDeveloperViewModel(Guid fileId, ExplicitConnection explicitConnection, IExplicitConnectionCache explicitConnectionCache, IHighlightingDefinition sqlHighlightingDefinition, ISnackbarMessageQueue snackbarMessageQueue, IDialogTargetFinder dialogTargetFinder)
        {
            if (explicitConnectionCache == null) throw new ArgumentNullException(nameof(explicitConnectionCache));

            FileId = fileId;
            _generalSettings = new GeneralSettings(10);
            _explicitConnection = explicitConnection;
            _explicitConnectionCache = explicitConnectionCache;

            FetchDocumentCommand = new Command(o => QueryRunnerViewModel.Run($"SELECT * FROM root r WHERE r.id = '{DocumentId}'"));
            EditConnectionCommand = new Command(sender => EditConnectionAsync((DependencyObject)sender));
            EditSettingsCommand = new Command(sender => EditSettingsAsync((DependencyObject)sender));
            QueryRunnerViewModel = new QueryRunnerViewModel(fileId, sqlHighlightingDefinition, () => _explicitConnection, () => _generalSettings, EditDocumentHandler, snackbarMessageQueue, dialogTargetFinder);
            DocumentEditorViewModel = new DocumentEditorViewModel(() => _explicitConnection, snackbarMessageQueue, dialogTargetFinder);            

            SetName();
        }

        public Guid FileId { get; }

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

        public Guid? ConnectionId => _explicitConnection?.Id;

        private void EditDocumentHandler(Result result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            DocumentEditorViewModel.Document.Text = result.Data;
            ViewIndex = 1;
        }

        private async void EditConnectionAsync(DependencyObject sender)
        {
            Debug.Assert(sender != null);

            var connectionOption = await new ConnectionManagementController(_explicitConnectionCache).Select(sender);

            SetConnection(connectionOption.ValueOr(() => _explicitConnection));
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

        private void SetConnection(ExplicitConnection explicitConnection)
        {
            _explicitConnection = explicitConnection;
            SetName();
        }

        private void SetName()
        {
            Name = _explicitConnection?.Label ?? "(no connection)";
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