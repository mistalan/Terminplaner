using System.Globalization;

namespace TerminplanerMaui.Converters;

public class DaysToShowConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int currentDays && parameter is string paramDays)
        {
            if (int.TryParse(paramDays, out int targetDays))
            {
                return currentDays == targetDays ? "#667eea" : "#9e9e9e";
            }
        }
        return "#9e9e9e";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
