using System.ComponentModel;
using Avalonia.Media;
using ecosystem.Models.Core;

namespace ecosystem.ViewModels;

public class EntityViewModel : ViewModelBase
{
    private readonly Entity _entity;
    private double _displayWidth;
    private double _displayHeight;

    public Entity Entity => _entity;

    public EntityViewModel(Entity entity)
    {
        _entity = entity;
        _entity.PropertyChanged += Entity_PropertyChanged;
    }

    public double DisplayX => _entity.Position.X * _displayWidth;
    public double DisplayY => _entity.Position.Y * _displayHeight;
    public IBrush? Color => _entity.Color;
    public string StatsText => _entity.Stats.DisplayStats;

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
        else if (e.PropertyName == nameof(Entity.StatsText))
        {
            OnPropertyChanged(nameof(StatsText));
        }
    }
}