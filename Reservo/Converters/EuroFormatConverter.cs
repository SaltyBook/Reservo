using System.Globalization;
using System.Windows.Data;

namespace Reservo.Converters
{
    public class EuroFormatConverter : IValueConverter
    {
        //Formats numeric values as a Euro currency string with two decimal places (N2 €).
        //Supports double values and parsable numeric strings.
        //Returns an empty string for null inputs and the original value otherwise.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";

            if (value is double d)
                return $"{d:N2} €";

            if (double.TryParse(value.ToString(), out double parsed))
                return $"{parsed:N2} €";

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString() == "")
                return null;
            return value;
        }
    }
}
