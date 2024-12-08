using System;
using Avalonia.Controls;

namespace ecosystem.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        try
        {
            InitializeComponent();
            Console.WriteLine("MainWindow initialized successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing MainWindow: {ex}");
            throw;
        }
    }
}