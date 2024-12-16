using System;
using System.ComponentModel;
using Avalonia.Media;
using ecosystem.Helpers;

namespace ecosystem.Models.Core;

public abstract class Entity : INotifyPropertyChanged
{
    private Position _position = null!;
    private IBrush? _color;
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

    public double GetDistanceTo(Position otherPosition)
    {
        return MathHelper.CalculateDistance(Position, otherPosition);
    }

    private void Position_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Console.WriteLine($"Position property changed: {e.PropertyName}");
        OnPropertyChanged($"Position.{e.PropertyName}");
        
        if (e.PropertyName == nameof(Position.X) || e.PropertyName == nameof(Position.Y))
        {
            OnPropertyChanged(nameof(Position));
        }
    }

    public IBrush? Color
    {
        get => _color;
        protected set
        {
            if (_color != value)
            {
                _color = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Color)));
            }
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