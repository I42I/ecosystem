using System.Collections.Generic;
using ecosystem.Models.Core;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;

namespace ecosystem.Models.Stats;

public class EntityStats : INotifyPropertyChanged
{
    private readonly LifeForm? _lifeForm;
    private string? _currentBehavior;
    public event PropertyChangedEventHandler? PropertyChanged;

    public EntityStats(LifeForm? lifeForm = null)
    {
        _lifeForm = lifeForm;
        Console.WriteLine($"Created EntityStats with lifeform: {_lifeForm}");
    }

    public string? CurrentBehavior 
    { 
        get => _currentBehavior;
        set 
        {
            Console.WriteLine($"Setting behavior for {_lifeForm?.GetType().Name}: {value}");
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
                
                if (!string.IsNullOrEmpty(CurrentBehavior))
                {
                    stats.Add($"B:{CurrentBehavior}");
                }
            }

            var result = string.Join(" | ", stats);
            Console.WriteLine($"[{_lifeForm?.GetType().Name}] Stats: {result}");
            return result;
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}