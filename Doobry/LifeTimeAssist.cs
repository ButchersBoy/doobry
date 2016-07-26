using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Doobry
{
    public static class LifeTimeAssist
    {
        public static readonly DependencyProperty RunOnLoadProperty = DependencyProperty.RegisterAttached(
            "RunOnLoad", typeof(ICommand), typeof(LifeTimeAssist), new PropertyMetadata(default(ICommand), RunOnLoadPropertyChangedCallback));

        private static void RunOnLoadPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var frameworkElement = dependencyObject as FrameworkElement;
            if (frameworkElement != null)
                frameworkElement.Loaded += (sender, args) => GetRunOnLoad(frameworkElement)?.Execute(dependencyObject);
        }

        public static void SetRunOnLoad(DependencyObject element, ICommand value)
        {
            element.SetValue(RunOnLoadProperty, value);
        }

        public static ICommand GetRunOnLoad(DependencyObject element)
        {
            return (ICommand) element.GetValue(RunOnLoadProperty);
        }

        public static readonly DependencyProperty RunOnWindowUnloadingProperty = DependencyProperty.RegisterAttached(
            "RunOnWindowUnloading", typeof(ICommand), typeof(LifeTimeAssist), new PropertyMetadata(default(ICommand), RunOnWindowClosingPropertyChangedCallback));

        private static void RunOnWindowClosingPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var window = dependencyObject as Window;
            if (window != null)
                window.Closing += (sender, args) => GetRunOnWindowUnloading(window)?.Execute(dependencyObject);
        }

        public static void SetRunOnWindowUnloading(DependencyObject element, ICommand value)
        {
            element.SetValue(RunOnWindowUnloadingProperty, value);
        }

        public static ICommand GetRunOnWindowUnloading(DependencyObject element)
        {
            return (ICommand) element.GetValue(RunOnWindowUnloadingProperty);
        }
    }
}
