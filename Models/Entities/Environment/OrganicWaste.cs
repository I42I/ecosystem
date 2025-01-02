using System;
using Avalonia.Media;
using ecosystem.Models.Core;
using ecosystem.Models.Radius;
using ecosystem.Services.Simulation;
using ecosystem.Services.World;

namespace ecosystem.Models.Entities.Environment;

public class OrganicWaste : Entity, IHasContactRange
{
    private int _energyValue;
    private readonly IWorldService _worldService;
    private double _decayAccumulator;

    public double ContactRadius { get; }

    public int EnergyValue 
    { 
        get => _energyValue;
        set
        {
            if (_energyValue != value)
            {
                _energyValue = Math.Max(0, value);
                OnPropertyChanged(nameof(EnergyValue));
                OnPropertyChanged(nameof(StatsText));
            }
        }
    }

    public override string StatsText => $"Energy: {EnergyValue}";

    protected override void OnPropertyChanged(string propertyName)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == nameof(EnergyValue))
        {
            OnPropertyChanged(nameof(StatsText));
        }
    }

    public OrganicWaste(Position position, int energyValue, IWorldService worldService)
        : base(position)
    {
        _worldService = worldService;
        EnergyValue = energyValue;
        Color = Brushes.Brown;
        ContactRadius = SimulationConstants.WASTE_CONTACT_RADIUS;
    }

    public override void Update()
    {
        _decayAccumulator += SimulationConstants.WASTE_DECAY_RATE;
        
        if (_decayAccumulator >= 1)
        {
            int decayAmount = (int)Math.Floor(_decayAccumulator);
            EnergyValue = Math.Max(0, EnergyValue - decayAmount);
            _decayAccumulator -= decayAmount;

            if (EnergyValue <= SimulationConstants.WASTE_MIN_ENERGY)
            {
                _worldService.RemoveEntity(this);
            }
        }
    }
}
