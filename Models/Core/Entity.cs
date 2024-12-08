using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Media;

namespace ecosystem.Models.Core;

public abstract class Entity : INotifyPropertyChanged
{
    private Position _position = null!;
    public Position Position
    {
        get => _position;
        protected set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            _position = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Position)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private IBrush _color = new SolidColorBrush(Colors.Black);
    public IBrush Color
    {
        get => _color;
        protected set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            _color = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Color)));
        }
    }

    protected Entity((double X, double Y) position)
    {
        if (position.X < 0 || position.Y < 0)
            throw new ArgumentException("Position must be non-negative", nameof(position));
            
        Position = new Position(position.X, position.Y);
        Console.WriteLine($"Created entity at position {Position.X}, {Position.Y}");
    }

    public virtual void Update() { }
}