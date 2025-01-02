using System;
using System.Collections.Generic;
using ecosystem.Models.Core;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Entities.Animals;

namespace ecosystem.Models.Stats;

public class EntityStats : INotifyPropertyChanged
{
    private string? _currentBehavior;
    private readonly Entity? _entity;
    private readonly LifeForm? _lifeForm;
    private string _displayStats = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public EntityStats(Entity? entity = null)
    {
        _entity = entity;
        _lifeForm = entity as LifeForm;
        
        if (_lifeForm != null)
        {
            _lifeForm.PropertyChanged += LifeForm_PropertyChanged;
        }

        if (_entity is OrganicWaste organicWaste)
        {
            organicWaste.PropertyChanged += OrganicWaste_PropertyChanged;
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

    private void OrganicWaste_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(OrganicWaste.EnergyValue))
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
            
            if (_entity is OrganicWaste organicWaste)
            {
                stats.Add($"E:{organicWaste.EnergyValue}");
            }
            else if (_lifeForm != null)
            {
                if (_entity is Animal animal)
                {
                    stats.Add($"({(animal.IsMale ? "M" : "F")})");
                }
                stats.Add($"HP:{_lifeForm.HealthPoints}");
                stats.Add($"E:{_lifeForm.Energy}");
            }

            var result = string.Join(" | ", stats);
            return result;
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}