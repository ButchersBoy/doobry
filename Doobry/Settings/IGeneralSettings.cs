using System.ComponentModel;

namespace Doobry.Settings
{
    public interface IGeneralSettings : INotifyPropertyChanged
    {
        int? MaxItemCount { get; set; }
    }
}