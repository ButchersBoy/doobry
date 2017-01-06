using System;
using System.Collections;
using System.ComponentModel;

namespace Doobry.Features.Management
{
    public class CreateItemProperties : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private string _itemId;
        private bool _hasErrors;
        private string _error;

        public CreateItemProperties(string typeDescription)
        {
            TypeDescription = typeDescription;
            ItemId = string.Empty;
        }

        public string TypeDescription { get; }

        public string ItemId
        {
            get { return _itemId; }
            set
            {
                this.MutateVerbose(ref _itemId, value, RaisePropertyChanged());
                Error = string.IsNullOrWhiteSpace(ItemId) ? "Required" : null;
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
                case nameof(ItemId):
                    if (string.IsNullOrWhiteSpace(ItemId))
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