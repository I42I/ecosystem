using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace ecosystem.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        try
        {
            AvaloniaXamlLoader.Load(this);
            Console.WriteLine("MainWindow initialized successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing MainWindow: {ex}");
            throw;
        }
    }
}