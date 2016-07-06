using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Doobry
{
    public class ResultSetExplorerViewModel : INotifyPropertyChanged
    {
        private int _selectedRow = -1;
        private ResultSet _resultSet;
        private bool _isError;
        private string _error;

        public ResultSetExplorerViewModel(ICommand fetchMoreCommand)
        {
            if (fetchMoreCommand == null) throw new ArgumentNullException(nameof(fetchMoreCommand));
            
            FetchMoreCommand = fetchMoreCommand;
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

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}
 