using System.Globalization;
using System.Windows.Data;

namespace Reservo.Converters
{
    public class ShortDateFormatConverter : IValueConverter
    {
        //Converts a DateTime value into a short year–month string (yy-MMM.) using German culture settings.
        //Returns an empty string if the input is not a DateTime.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dt)
                return dt.ToString("yy-MMM.", CultureInfo.GetCultureInfo("de-DE"));
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
