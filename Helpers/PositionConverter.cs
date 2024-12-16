using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using System.Collections.Generic;
using ecosystem.Models.Core;

namespace ecosystem.Helpers;

public class PositionConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count != 3 || 
            values[0] is not Position position || 
            values[1] is UnsetValueType || 
            values[2] is UnsetValueType)
        {
            return new Point(0, 0);
        }

        try
        {
            double width = System.Convert.ToDouble(values[1]);
            double height = System.Convert.ToDouble(values[2]);
            
            return new Point(
                position.X * width,
                position.Y * height
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PositionConverter error: {ex.Message}");
            return new Point(0, 0);
        }
    }

    public object[]? ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}