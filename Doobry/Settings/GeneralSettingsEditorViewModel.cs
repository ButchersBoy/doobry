using System;
using System.ComponentModel;

namespace Doobry.Settings
{
    public class GeneralSettingsEditorViewModel : INotifyPropertyChanged
    {
        private int? _maxItemCount;

        public int? MaxItemCount
        {
            get { return _maxItemCount; }
            set { this.MutateVerbose(ref _maxItemCount, value, RaisePropertyChanged()); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}
