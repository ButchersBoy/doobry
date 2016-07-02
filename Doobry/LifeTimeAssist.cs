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
                frameworkElement.Loaded += (sender, args) => GetRunOnLoad(frameworkElement)?.Execute(null);
        }

        public static void SetRunOnLoad(DependencyObject element, ICommand value)
        {
            element.SetValue(RunOnLoadProperty, value);
        }

        public static ICommand GetRunOnLoad(DependencyObject element)
        {
            return (ICommand) element.GetValue(RunOnLoadProperty);
        }

        public static readonly DependencyProperty RunOnUnloadProperty = DependencyProperty.RegisterAttached(
            "RunOnUnload", typeof(ICommand), typeof(LifeTimeAssist), new PropertyMetadata(default(ICommand), RunOnUnloadPropertyChangedCallback));

        private static void RunOnUnloadPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var frameworkElement = dependencyObject as FrameworkElement;
            if (frameworkElement != null)
                frameworkElement.Loaded += (sender, args) => GetRunOnUnload(frameworkElement)?.Execute(null);
        }

        public static void SetRunOnUnload(DependencyObject element, ICommand value)
        {
            element.SetValue(RunOnUnloadProperty, value);
        }

        public static ICommand GetRunOnUnload(DependencyObject element)
        {
            return (ICommand) element.GetValue(RunOnUnloadProperty);
        }
    }
}
