using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Doobry.Infrastructure;

namespace Doobry.Features.QueryDeveloper
{
    public class ResultSetExplorerViewModel : INotifyPropertyChanged
    {
        private int _selectedRow = -1;
        private ResultSet _resultSet;
        private bool _isError;
        private string _error;

        public ResultSetExplorerViewModel(ICommand fetchMoreCommand, ICommand editDocumentCommand,
            ICommand deleteDocumentCommand)
        {
            if (fetchMoreCommand == null) throw new ArgumentNullException(nameof(fetchMoreCommand));

            FetchMoreCommand = fetchMoreCommand;
            EditDocumentCommand = editDocumentCommand;
            DeleteDocumentCommand = deleteDocumentCommand;
            SaveDocumentCommand = new Command(o => SaveDocument((Result) o), o => o is Result);
        }

        public ResultSet ResultSet
        {
            get { return _resultSet; }
            set
            {
                this.MutateVerbose(ref _resultSet, value, RaisePropertyChanged());

                if (_resultSet != null)
                {
                    if (!string.IsNullOrEmpty(_resultSet.Error))
                    {
                        IsError = true;
                        Error = _resultSet.Error;
                        SelectedRow = -1;
                    }
                    else
                    {
                        IsError = false;
                        Error = null;
                        if (_resultSet.Results.Count > 0)
                            SelectedRow = 0;
                        else
                            SelectedRow = -1;
                    }
                }
                else
                    SelectedRow = -1;
            }
        }

        public ICommand FetchMoreCommand { get; }

        public ICommand DeleteDocumentCommand { get; }

        public ICommand EditDocumentCommand { get; }

        public ICommand SaveDocumentCommand { get; }

        public bool IsError
        {
            get { return _isError; }
            private set { this.MutateVerbose(ref _isError, value, RaisePropertyChanged()); }
        }

        public string Error
        {
            get { return _error; }
            private set { this.MutateVerbose(ref _error, value, RaisePropertyChanged()); }
        }

        public int SelectedRow
        {
            get { return _selectedRow; }
            set { this.MutateVerbose(ref _selectedRow, value, RaisePropertyChanged()); }
        }

        private void SaveDocument(Result result)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "Document",
                DefaultExt = ".json",
                Filter = "JSON documents (.json)|*.json|Text documents (.txt)|*.txt"
            };

            var showDialogResult = saveFileDialog.ShowDialog();
            if (!showDialogResult.HasValue || !showDialogResult.Value) return;
            try
            {
                File.WriteAllText(saveFileDialog.FileName, result.Data);
            }
            catch (Exception)
            {
                //TODO show a dialog!
                throw;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}
 