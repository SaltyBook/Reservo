using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Reservo.Converters
{
    public class NullVisibilityConverter : IMultiValueConverter
    {
        //Formats a DateTime value into a full, human-readable date string (dddd, dd MMMM, yyyy) using German culture.
        //Returns an empty string for non-DateTime values.
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is null) return Visibility.Hidden;
            bool.TryParse(values[1].ToString(), out bool result);
            if (values[0].ToString() == "" && !result) return Visibility.Hidden;
            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
