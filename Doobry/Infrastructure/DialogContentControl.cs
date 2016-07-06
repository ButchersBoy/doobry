using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace Doobry.Infrastructure
{    
    public class DialogContentControl : ContentControl
    {
        static DialogContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogContentControl), new FrameworkPropertyMetadata(typeof(DialogContentControl)));
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(DialogContentControl), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", typeof(PackIconKind), typeof(DialogContentControl), new PropertyMetadata(default(PackIconKind)));

        public PackIconKind Icon
        {
            get { return (PackIconKind) GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
    }
}
