using System;
using Avalonia.Media;
using ecosystem.Models.Core;

namespace ecosystem.Models.Entities.Environment;

public class OrganicWaste : Entity
{
    private int _energyValue;
    public int EnergyValue 
    { 
        get => _energyValue;
        set => _energyValue = Math.Max(0, value);
    }

    public OrganicWaste(Position position, int energyValue)
        : base(position)
    {
        EnergyValue = energyValue;
        Color = Brushes.Brown;
    }
}
