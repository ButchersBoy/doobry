using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Doobry.Settings
{
    public class GeneralSettings : IGeneralSettings
    {
        private int? _maxItemCount;

        public GeneralSettings(int? maxItemCount)
        {
            MaxItemCount = maxItemCount;
        }

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