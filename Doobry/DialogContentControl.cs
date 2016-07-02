using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;

namespace Doobry
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
