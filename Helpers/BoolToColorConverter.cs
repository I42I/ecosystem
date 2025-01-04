using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace ecosystem.Helpers;

public class BoolToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isActive && parameter is string colors)
        {
            var parts = colors.Split('|');
            if (parts.Length == 2)
            {
                string colorStr = isActive ? parts[0] : parts[1];
                return Color.Parse(colorStr);
            }
        }
        return Colors.Gray;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}