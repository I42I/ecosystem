using Avalonia;
using Avalonia.Controls;
using System;

namespace ecosystem.Helpers;

public static class SizeObserver
{
    public static readonly AttachedProperty<bool> ObserveProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("Observe", typeof(SizeObserver));

    public static readonly AttachedProperty<double> ObservedWidthProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("ObservedWidth", typeof(SizeObserver));

    public static readonly AttachedProperty<double> ObservedHeightProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("ObservedHeight", typeof(SizeObserver));

    static SizeObserver()
    {
        ObserveProperty.Changed.Subscribe(OnObservePropertyChanged);
    }

    private static void OnObservePropertyChanged(AvaloniaPropertyChangedEventArgs<bool> e)
    {
        if (e.Sender is Control control)
        {
            if (e.NewValue.GetValueOrDefault())
            {
                control.LayoutUpdated += Control_LayoutUpdated;
                UpdateObservedSizes(control);
            }
            else
            {
                control.LayoutUpdated -= Control_LayoutUpdated;
            }
        }
    }

    private static void Control_LayoutUpdated(object? sender, EventArgs e)
    {
        if (sender is Control control)
        {
            UpdateObservedSizes(control);
        }
    }

    private static void UpdateObservedSizes(Control control)
    {
        var width = control.Bounds.Width;
        var height = control.Bounds.Height;
        Console.WriteLine($"Canvas size updated: {width}x{height}");
        control.SetValue(ObservedWidthProperty, control.Bounds.Width);
        control.SetValue(ObservedHeightProperty, control.Bounds.Height);
    }

    public static void SetObserve(AvaloniaObject element, bool value)
    {
        element.SetValue(ObserveProperty, value);
    }

    public static bool GetObserve(AvaloniaObject element)
    {
        return element.GetValue(ObserveProperty);
    }

    public static double GetObservedWidth(AvaloniaObject element)
    {
        return element.GetValue(ObservedWidthProperty);
    }

    public static double GetObservedHeight(AvaloniaObject element)
    {
        return element.GetValue(ObservedHeightProperty);
    }
}