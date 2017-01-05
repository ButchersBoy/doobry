using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Doobry.Infrastructure
{
    public class NullableToVisibilityConverter : IValueConverter
    {
        public Visibility NotNullValue { get; set; } = Visibility.Visible;

        public Visibility NullValue { get; set; } = Visibility.Collapsed;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ReferenceEquals(value, null) ? NullValue : NotNullValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}