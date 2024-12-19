using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using ecosystem.ViewModels;

namespace ecosystem.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void Window_Loaded(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            vm.WindowWidth = ClientSize.Width;
            vm.WindowHeight = ClientSize.Height;
            Console.WriteLine($"Initial window size: {ClientSize.Width}x{ClientSize.Height}");
        }
    }

    private void Window_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            vm.WindowWidth = e.NewSize.Width;
            vm.WindowHeight = e.NewSize.Height;
            Console.WriteLine($"Window size changed to {e.NewSize.Width}x{e.NewSize.Height}");
        }
    }
}