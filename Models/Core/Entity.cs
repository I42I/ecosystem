using System;
using System.ComponentModel;
using Avalonia.Media;
using ecosystem.Helpers;
using System.Collections.Generic;
using ecosystem.Models.Stats;

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

    private void Position_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged($"Position.{e.PropertyName}");
    }

    public double GetDistanceTo(Position otherPosition)
    {
        return MathHelper.CalculateDistance(Position, otherPosition);
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

    private static readonly Dictionary<Type, int> _typeCounters = new();
    private readonly int _id;
    public EntityStats Stats { get; }
    
    public string DisplayName => $"{GetType().Name} {_id}";
    public int TypeId => _id;

    public string StatsText => Stats?.DisplayStats ?? string.Empty;

    protected Entity(Position position)
    {
        lock (_typeCounters)
        {
            var type = GetType();
            if (!_typeCounters.ContainsKey(type))
            {
                _typeCounters[type] = 0;
            }
            _id = ++_typeCounters[type];
        }

        if (position is null)
            throw new ArgumentNullException(nameof(position));
        if (position.X < 0 || position.Y < 0)
            throw new ArgumentException("Position must be non-negative", nameof(position));
            
        Position = position;
        Stats = new EntityStats(this as LifeForm);
        // Console.WriteLine($"Created entity at position {Position.X}, {Position.Y}");
    }

    public virtual void Update() { }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        // Console.WriteLine($"Property changed: {propertyName} for {GetType().Name}");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}