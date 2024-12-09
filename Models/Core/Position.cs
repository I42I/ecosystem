using System;
using System.ComponentModel;

namespace ecosystem.Models.Core;

public class Position : INotifyPropertyChanged
{
    private double _x;
    private double _y;

    public Position(double x, double y)
    {
        Console.WriteLine($"Creating position at ({x}, {y})");
        X = x;
        Y = y;
    }

    public double X
    {
        get => _x;
        set
        {
            if (_x != value)
            {
                _x = value;
                Console.WriteLine($"Position X changed to {value}");
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(X)));
            }
        }
    }

    public double Y 
    {
        get => _y;
        set
        {
            if (_y != value)
            {
                _y = value;
                Console.WriteLine($"Position Y changed to {value}");
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Y)));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
