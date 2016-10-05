using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace Doobry.Infrastructure
{
    public class DialogHeader : Control
    {
        static DialogHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogHeader), new FrameworkPropertyMetadata(typeof(DialogHeader)));
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", typeof(PackIconKind), typeof(DialogHeader), new PropertyMetadata(default(PackIconKind)));

        public PackIconKind Icon
        {
            get { return (PackIconKind) GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(DialogHeader), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
    }
}