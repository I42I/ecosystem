using System.Collections.Generic;
using ecosystem.Models.Core;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;

namespace ecosystem.Models.Stats;

public class EntityStats : INotifyPropertyChanged
{
    private string? _currentBehavior;
    private readonly LifeForm? _lifeForm;
    private string _displayStats = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public EntityStats(LifeForm? lifeForm = null)
    {
        _lifeForm = lifeForm;
        if (_lifeForm != null)
        {
            _lifeForm.PropertyChanged += LifeForm_PropertyChanged;
        }
    }

    private void LifeForm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LifeForm.HealthPoints) || 
            e.PropertyName == nameof(LifeForm.Energy))
        {
            OnPropertyChanged(nameof(DisplayStats));
        }
    }

    public string? CurrentBehavior 
    { 
        get => _currentBehavior;
        set 
        {
            if (_currentBehavior != value)
            {
                _currentBehavior = value;
                OnPropertyChanged(nameof(CurrentBehavior));
                OnPropertyChanged(nameof(DisplayStats));
            }
        }
    }

    public string DisplayStats
    {
        get
        {
            var stats = new List<string>();
            
            if (_lifeForm != null)
            {
                stats.Add($"HP:{_lifeForm.HealthPoints}");
                stats.Add($"E:{_lifeForm.Energy}");
            }

            return string.Join(" | ", stats);
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}