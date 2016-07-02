using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DocumentDbIx
{
    public class ConnectionEditorViewModel : INotifyPropertyChanged
    {
        private string _host = "https://c64.documents.azure.com:443/";
        private string _authorisationKey = "BEZfZWprSG5a6SVMCXvNJClfx3Nchk7sASMX6S8LRnCSSeh5dPdPM9657jsQHqdaMlOjMlZCw1eaMOzCWCgCNg==";
        private string _databaseId = "quotesystem";
        private string _collectionId = "california";

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
