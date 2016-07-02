using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Doobry
{
    public class ConnectionEditorViewModel : INotifyPropertyChanged
    {
        private string _host;
        private string _authorisationKey;
        private string _databaseId;
        private string _collectionId;

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

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}
