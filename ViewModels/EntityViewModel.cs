using System;
using System.ComponentModel;
using Avalonia.Media;
using ecosystem.Models.Core;

namespace ecosystem.ViewModels;

public class EntityViewModel : ViewModelBase
{
    private readonly Entity _entity;
    private double _displayWidth = 800;
    private double _displayHeight = 600; 

    public Entity Entity => _entity;

    public EntityViewModel(Entity entity)
    {
        _entity = entity;
        _entity.PropertyChanged += Entity_PropertyChanged;
        _entity.Stats.PropertyChanged += Stats_PropertyChanged;
    }

    public double DisplayX => _entity.Position.X * _displayWidth;
    public double DisplayY => _entity.Position.Y * _displayHeight;
    public IBrush? Color => _entity.Color;
    public string StatsText
    {
        get
        {
            var stats = $"{_entity.Stats.DisplayStats}";
            var behavior = _entity.Stats.CurrentBehavior;
            return $"{stats}\n{behavior ?? "None"}";
        }
    }

    public void UpdateDisplaySize(double width, double height)
    {
        _displayWidth = width;
        _displayHeight = height;
        OnPropertyChanged(nameof(DisplayX));
        OnPropertyChanged(nameof(DisplayY));
    }

    private void Entity_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Entity.Position))
        {
            OnPropertyChanged(nameof(DisplayX));
            OnPropertyChanged(nameof(DisplayY));
        }
        else if (e.PropertyName == nameof(Entity.Stats))
        {
            OnPropertyChanged(nameof(StatsText));
        }
    }

    private void Stats_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Entity.Stats.DisplayStats) ||
            e.PropertyName == nameof(Entity.Stats.CurrentBehavior))
        {
            // Console.WriteLine($"Stats changed for {Entity.DisplayName}: {StatsText}");
            OnPropertyChanged(nameof(StatsText));
        }
    }
}