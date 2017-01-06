using System;
using System.Collections;
using System.ComponentModel;

namespace Doobry.Features.Management
{
    public class DeleteItemProperties : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private bool _isConfirmed;
        private bool _hasErrors;

        public DeleteItemProperties(string typeDescription, string description)
        {
            TypeDescription = typeDescription;
            Description = description;
            HasErrors = !IsConfirmed;
        }

        public string TypeDescription { get; }

        public string Description { get; }

        public bool IsConfirmed
        {
            get { return _isConfirmed; }
            set
            {
                this.MutateVerbose(ref _isConfirmed, value, RaisePropertyChanged());                
                HasErrors = !IsConfirmed;
            }
        }

        public bool HasErrors
        {
            get { return _hasErrors; }
            private set { this.MutateVerbose(ref _hasErrors, value, RaisePropertyChanged()); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            yield break;
        }

        protected virtual void OnErrorsChanged(DataErrorsChangedEventArgs e)
        {
            ErrorsChanged?.Invoke(this, e);
        }

    }
}