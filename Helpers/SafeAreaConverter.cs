using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ecosystem.Helpers;

public class SafeAreaConverter : IValueConverter
{
    private const double TOP_MARGIN = 30.0;
    private const double BOTTOM_MARGIN = 50.0;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double size)
        {
            if (parameter?.ToString() == "height")
            {
                return size + TOP_MARGIN;
            }
            return size;
        }
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class SafeAreaMarginConverter : IValueConverter
{
    private const double TOP_MARGIN = 30.0;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return new Avalonia.Thickness(0, 0, 0, 0);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}