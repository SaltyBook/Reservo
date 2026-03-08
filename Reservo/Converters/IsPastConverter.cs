using System.Globalization;
using System.Windows.Data;

namespace Reservo.Converters
{
    public class IsPastConverter : IValueConverter
    {
        //Checks whether the given DateTime value represents a date in the past (earlier than today).
        //Returns false if the input is not a valid DateTime.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dt)
            {
                return dt.Date < DateTime.Today;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
