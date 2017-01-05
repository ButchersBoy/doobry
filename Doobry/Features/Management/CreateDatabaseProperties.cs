using System;
using System.Collections;
using System.ComponentModel;

namespace Doobry.Features.Management
{
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
}