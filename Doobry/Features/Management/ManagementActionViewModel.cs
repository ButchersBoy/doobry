using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Doobry.Infrastructure;
using DynamicData.Binding;

namespace Doobry.Features.Management
{
    public class ManagementActionViewModel<TProperties> : INotifyPropertyChanged, IDisposable
        where TProperties : INotifyDataErrorInfo, INotifyPropertyChanged
    {
        private readonly Func<TProperties, Task> _taskFactory;
        private readonly Action _onEnd;
        private readonly IDisposable _disposable;
        private ManagementActionStep _step;
        private string _error;

        public ManagementActionViewModel(TProperties propertiesViewModel, Func<TProperties, Task> taskFactory, Action onEnd)
        {
            _taskFactory = taskFactory;
            _onEnd = onEnd;
            PropertiesViewModel = propertiesViewModel;
            
            var okCommand = new Command(_ => MangeRun(), _ => !propertiesViewModel.HasErrors);
            _disposable = propertiesViewModel.WhenPropertyChanged(p => p.HasErrors).Subscribe(v => okCommand.Refresh());

            OkCommand = okCommand;
            CancelCommand = new Command(_ => onEnd());
            DismissErrorCommand = new Command(_ => Step = ManagementActionStep.CollectInput);
        }

        public ManagementActionStep Step
        {
            get { return _step; }
            private set { this.MutateVerbose(ref _step, value, RaisePropertyChanged()); }
        }

        public ICommand OkCommand { get; }

        public ICommand CancelCommand { get; }

        public ICommand DismissErrorCommand { get; }

        public TProperties PropertiesViewModel { get; }

        private void MangeRun()
        {
            Step = ManagementActionStep.Run;
            _taskFactory(PropertiesViewModel)
                    .ContinueWith(t =>
                    {
                        if (t.Exception != null)
                        {
                            Step = ManagementActionStep.ReportFailure;
                            Error = t.Exception.ToString();
                        }
                        else
                            _onEnd();
                    }, TaskScheduler.FromCurrentSynchronizationContext());
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

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}