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
            var de = new CultureInfo("de-DE");

            if (value == null)
                return string.Empty;

            if (value is double d)
                return d.ToString("N2", de) + " €";

            if (value is decimal dec)
                return dec.ToString("N2", de) + " €";

            var text = value.ToString()?.Trim();
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            text = text.Replace("€", "").Trim();

            if (double.TryParse(text, NumberStyles.Any, de, out var parsed))
                return parsed.ToString("N2", de) + " €";

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var de = new CultureInfo("de-DE");

            if (value == null)
                return 0d;

            var text = value.ToString()?.Trim();
            if (string.IsNullOrWhiteSpace(text))
                return 0d;

            text = text.Replace("€", "").Trim();

            if (double.TryParse(text, NumberStyles.Any, de, out var parsed))
                return parsed;

            return Binding.DoNothing;
        }
    }
}
