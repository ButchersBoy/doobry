using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Doobry.Infrastructure
{
    public static class FocusAssist
    {
        public static readonly DependencyProperty IsViableTargetProperty = DependencyProperty.RegisterAttached(
            "IsViableTarget", typeof(bool), typeof(FocusAssist), new PropertyMetadata(default(bool)));

        public static void SetIsViableTarget(DependencyObject element, bool value)
        {
            element.SetValue(IsViableTargetProperty, value);
        }

        public static bool GetIsViableTarget(DependencyObject element)
        {
            return (bool) element.GetValue(IsViableTargetProperty);
        }

        public static void FocusViableTarget(DependencyObject ancestor)
        {
            if (ancestor == null) throw new ArgumentNullException(nameof(ancestor));

            ancestor.Dispatcher.BeginInvoke(new Action(() =>
            {
                var dependencyObject =
                    ancestor.VisualDepthFirstTraversal()
                        .FirstOrDefault(
                            depObj =>
                                GetIsViableTarget(depObj) &&
                                depObj.GetVisualAncestry()
                                    .OfType<FrameworkElement>()
                                    .All(checkAncestor => checkAncestor.IsLoaded && checkAncestor.IsVisible)) as FrameworkElement;
                dependencyObject?.Focus();
            }), DispatcherPriority.Background);
        }
    }
}
