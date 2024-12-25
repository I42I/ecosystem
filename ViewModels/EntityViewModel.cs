using System;
using System.ComponentModel;
using Avalonia.Media;
using ecosystem.Models.Core;
using ecosystem.Models.Radius;

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

    public double ScaledVisionRadius => Entity is IHasVisionRange e ? e.VisionRadius * _displayWidth : 0;
    public double ScaledContactRadius => Entity is IHasContactRange e ? e.ContactRadius * _displayWidth : 0;
    public double ScaledRootRadius => Entity is IHasRootSystem e ? e.RootRadius * _displayWidth : 0;
    public double CenteredX => DisplayX - ScaledVisionRadius / 2;
    public double CenteredY => DisplayY - ScaledVisionRadius / 2;
    
    public double CenteredContactX => DisplayX - ScaledContactRadius / 2;
    public double CenteredContactY => DisplayY - ScaledContactRadius / 2;
    
    public double CenteredRootX => DisplayX - ScaledRootRadius / 2;
    public double CenteredRootY => DisplayY - ScaledRootRadius / 2;

    public void UpdateDisplaySize(double width, double height)
    {
        _displayWidth = width;
        _displayHeight = height;
        OnPropertyChanged(nameof(DisplayX));
        OnPropertyChanged(nameof(DisplayY));
        OnPropertyChanged(nameof(ScaledVisionRadius));
        OnPropertyChanged(nameof(ScaledContactRadius));
        OnPropertyChanged(nameof(ScaledRootRadius));
        OnPropertyChanged(nameof(CenteredX));
        OnPropertyChanged(nameof(CenteredY));
        OnPropertyChanged(nameof(CenteredContactX));
        OnPropertyChanged(nameof(CenteredContactY));
        OnPropertyChanged(nameof(CenteredRootX));
        OnPropertyChanged(nameof(CenteredRootY));
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
            OnPropertyChanged(nameof(StatsText));
        }
    }
}