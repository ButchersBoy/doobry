using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Doobry.Infrastructure
{
    public class DispatcherTaskSchedulerProvider
    {
        public static DispatcherTaskSchedulerProvider Create(Dispatcher dispatcher)
        {
            TaskScheduler taskScheduler = null;
            dispatcher.Invoke(() => taskScheduler = TaskScheduler.FromCurrentSynchronizationContext());
            return new DispatcherTaskSchedulerProvider(taskScheduler);
        }

        private DispatcherTaskSchedulerProvider(TaskScheduler taskScheduler)
        {
            TaskScheduler = taskScheduler;
        }

        public TaskScheduler TaskScheduler { get; }
    }
}
