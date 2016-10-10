using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Doobry.Settings
{
    public class AutoSaver : IDisposable
    {        
        private readonly IDisposable _disposable;

        public AutoSaver(IConnectionCache connectionCache, IGeneralSettings generalSettings, IManualSaver manualSaver)
        {
            if (connectionCache == null) throw new ArgumentNullException(nameof(connectionCache));
            if (generalSettings == null) throw new ArgumentNullException(nameof(generalSettings));
            if (manualSaver == null) throw new ArgumentNullException(nameof(manualSaver));            

            _disposable = connectionCache.Connect()
                .Select(_ => Unit.Default)
                .Merge(generalSettings.OnAnyPropertyChanged().Select(_ => Unit.Default))                
                .Throttle(TimeSpan.FromSeconds(2))
                .ObserveOn(new EventLoopScheduler())                
                .Subscribe(_ => manualSaver.Save(connectionCache, generalSettings));
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
