using System.Windows;

namespace Doobry.Infrastructure
{
    public class ConfirmationDialog : MessageDialog
    {
        static ConfirmationDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ConfirmationDialog), new FrameworkPropertyMetadata(typeof(ConfirmationDialog)));
        }
    }
}