using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doobry
{
    public class ResultSetExplorerViewModel : INotifyPropertyChanged
    {
        private int _selectedRow = -1;
        private ResultSet _resultSet;

        public ResultSet ResultSet
        {
            get { return _resultSet; }
            set
            {
                this.MutateVerbose(ref _resultSet, value, RaisePropertyChanged());                
                if (_resultSet.Results.Count > 0)
                    SelectedRow = 0;
                else
                    SelectedRow = -1;
            }
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
