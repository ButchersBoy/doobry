using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Doobry.Infrastructure
{
    public class CancellableDialogViewModel : INotifyPropertyChanged
    {
        private bool _isCancelAllowed;
        private readonly Command _cancelCommand;

        public CancellableDialogViewModel(Action cancelHandler, TimeSpan enableCancelDelay, Dispatcher dispatcher)
        {
            if (cancelHandler == null) throw new ArgumentNullException(nameof(cancelHandler));
            if (dispatcher == null) throw new ArgumentNullException(nameof(dispatcher));

            new DispatcherTimer(enableCancelDelay, DispatcherPriority.Normal, (sender, args) =>
            {
                IsCancelAllowed = true;
                ((DispatcherTimer) sender).Stop();
            },
                dispatcher).Start();
             
            _cancelCommand = new Command(o => cancelHandler(), _ => IsCancelAllowed);
        }

        public bool IsCancelAllowed
        {
            get { return _isCancelAllowed; }
            private set
            {
                this.MutateVerbose(ref _isCancelAllowed, value, RaisePropertyChanged());
                _cancelCommand.Refresh();                
            }
        }

        public ICommand CancelCommand => _cancelCommand;


        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}
