using System.Globalization;
using DrasticAI.Models;

namespace PhiExplorer.Converters;

public class ParameterTypeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ParameterType type)
        {
            return type.ToString();
        }

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}