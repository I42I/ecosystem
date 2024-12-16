using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using System.Collections.Generic;

namespace ecosystem.Helpers;

public class MultiplyConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2 || values[0] is UnsetValueType || values[1] is UnsetValueType)
        {
            Console.WriteLine("MultiplyConverter: Waiting for values to be set");
            return 0;
        }

        try
        {
            double value = System.Convert.ToDouble(values[0]);
            double factor = System.Convert.ToDouble(values[1]);
            double result = value * factor;
            Console.WriteLine($"MultiplyConverter: {value} * {factor} = {result}");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"MultiplyConverter error: {ex.Message}");
            return 0;
        }
    }

    public object[]? ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}