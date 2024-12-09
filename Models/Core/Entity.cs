using System;
using System.ComponentModel;
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
            
            if (_position != null)
                _position.PropertyChanged -= Position_PropertyChanged;
                
            _position = value;
            _position.PropertyChanged += Position_PropertyChanged;
            OnPropertyChanged(nameof(Position));
        }
    }

    private void Position_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        Console.WriteLine($"Position property changed: {e.PropertyName}");
        OnPropertyChanged($"Position.{e.PropertyName}");
        
        if (e.PropertyName == nameof(Position.X) || e.PropertyName == nameof(Position.Y))
        {
            OnPropertyChanged(nameof(Position));
        }
    }

    private IBrush _color = new SolidColorBrush(Colors.Black);
    public IBrush Color
    {
        get => _color;
        protected set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            _color = value;
            OnPropertyChanged(nameof(Color));
        }
    }

    protected Entity(Position position)
    {
        if (position is null)
            throw new ArgumentNullException(nameof(position));
        if (position.X < 0 || position.Y < 0)
            throw new ArgumentException("Position must be non-negative", nameof(position));
            
        Position = position;
        // Console.WriteLine($"Created entity at position {Position.X}, {Position.Y}");
    }

    public virtual void Update() { }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}