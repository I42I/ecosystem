using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ecosystem.Helpers;

public class RadiusConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double radius && parameter is string scale)
        {
            double displayScale = double.Parse(scale);
            return radius * displayScale;
        }
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}