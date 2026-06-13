using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Reservo.Converters
{
    public class BarColorConverter : IValueConverter
    {
        public string Mode { get; set; } = "Background";

        private static readonly Dictionary<string, (string bg, string fg)> Colors = new()
        {
            { "blue",   ("#B5D4F4", "#0C447C") },
            { "teal",   ("#9FE1CB", "#085041") },
            { "purple", ("#CECBF6", "#26215C") },
            { "coral",  ("#F5C4B3", "#4A1B0C") },
            { "green",  ("#C0DD97", "#173404") },
            { "amber",  ("#FAC775", "#633806") },
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var key = value as string ?? "blue";
            if (!Colors.TryGetValue(key, out var pair))
                pair = Colors["blue"];

            var hex = Mode == "Foreground" ? pair.fg : pair.bg;
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
