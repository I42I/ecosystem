using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ecosystem.Helpers;

public class OffsetConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double position && parameter is string offset)
        {
            return position + double.Parse(offset);
        }
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}