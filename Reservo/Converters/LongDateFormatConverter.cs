using System.Globalization;
using System.Windows.Data;

namespace Reservo.Converters
{
    public class LongDateFormatConverter : IValueConverter
    {
        //Formats a DateTime value into a full, human-readable date string (dddd, dd MMMM, yyyy) using German culture.
        //Returns an empty string for non-DateTime values.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dt)
                return dt.ToString("dddd, dd MMMM, yyyy", CultureInfo.GetCultureInfo("de-DE"));
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
