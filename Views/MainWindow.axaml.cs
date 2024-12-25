using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using ecosystem.ViewModels;

namespace ecosystem.Views;

public partial class MainWindow : Window
{
    private const double TOP_MARGIN = 30;
    private const double BOTTOM_MARGIN = 50;

    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void Window_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            double adjustedHeight = e.NewSize.Height - TOP_MARGIN - BOTTOM_MARGIN;
            vm.WindowWidth = e.NewSize.Width;
            vm.WindowHeight = adjustedHeight;
            Console.WriteLine($"Window size changed to {e.NewSize.Width}x{adjustedHeight} (original: {e.NewSize.Height})");
        }
    }

    private void Window_Loaded(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            double adjustedHeight = ClientSize.Height - TOP_MARGIN - BOTTOM_MARGIN;
            vm.WindowWidth = ClientSize.Width;
            vm.WindowHeight = adjustedHeight;
            Console.WriteLine($"Initial window size: {ClientSize.Width}x{adjustedHeight}");
        }
    }
}