using System;
using Avalonia.Media;
using ecosystem.Models.Core;
using ecosystem.Models.Radius;
using ecosystem.Services.Simulation;
using ecosystem.Services.World;
using ecosystem.Models.Animation;
using ecosystem.Helpers;

namespace ecosystem.Models.Entities.Environment;

public class OrganicWaste : Entity, IHasContactRange, IStaticSpriteEntity
{
    public ISprite? Sprite { get; private set; }
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
            }
        }
    }

    public OrganicWaste(Position position, int energyValue, IWorldService worldService)
        : base(position)
    {
        _worldService = worldService;
        EnergyValue = energyValue;
        Color = Brushes.Brown;
        ContactRadius = SimulationConstants.WASTE_CONTACT_RADIUS;

        try
        {
            Sprite = new StaticSprite(AssetHelper.GetAssetPath("Melon_Seeds_JE2.png"));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load organic waste sprite: {ex.Message}");
        }
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
