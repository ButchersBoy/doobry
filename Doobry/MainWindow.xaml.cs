using Doobry.Infrastructure;
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
using System.Windows.Threading;
using MaterialDesignThemes.Wpf.Transitions;

namespace Doobry
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Transitioner_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var transitioner = (Transitioner) sender;
            var transitionerSlides = LogicalTreeHelper.GetChildren(transitioner).OfType<DependencyObject>().ToList();
            FocusAssist.FocusViableTarget(transitionerSlides[transitioner.SelectedIndex]);
        }
    }
}
