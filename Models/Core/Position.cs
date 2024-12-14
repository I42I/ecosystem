using System;
using System.ComponentModel;

namespace ecosystem.Models.Core;

public class Position : INotifyPropertyChanged
{
    private double _x;
    private double _y;

    public double X
    {
        get => _x;
        set
        {
            if (_x != value)
            {
                _x = value;
                // Console.WriteLine($"Position X changed to {value}");
                OnPropertyChanged(nameof(X));
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
                // Console.WriteLine($"Position Y changed to {value}");
                OnPropertyChanged(nameof(Y));
            }
        }
    }

    public Position(double x, double y)
    {
        X = x;
        Y = y;
    }

    public Position() : this(0, 0) 
    {
    }

    public static Position operator -(Position a, Position b)
    {
        return new Position(a.X - b.X, a.Y - b.Y);
    }

    public static Position operator /(Position p, double scalar)
    {
        return new Position(p.X / scalar, p.Y / scalar);
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
