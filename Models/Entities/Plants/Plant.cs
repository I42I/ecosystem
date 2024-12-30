using System;
using System.Linq;
using System.Collections.Generic;
using ecosystem.Helpers;
using ecosystem.Models.Core;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Behaviors;
using ecosystem.Services.World;
using ecosystem.Services.Simulation;
using ecosystem.Models.Radius;

namespace ecosystem.Models.Entities.Plants;

public abstract class Plant : LifeForm, IHasRootSystem
{
    private double _growthAccumulator;
    private double _reproductionAccumulator;
    protected abstract double BaseAbsorptionRate { get; }
    protected readonly IWorldService _worldService;
    public abstract EnvironmentType PreferredEnvironment { get; }
    public double RootRadius { get; protected set; }
    public double SeedRadius { get; protected set; }

    protected Plant(
        int healthPoints,
        int energy,
        Position position,
        double basalMetabolicRate,
        EnvironmentType environment,
        double rootRadius,
        double seedRadius,
        double contactRadius,
        IWorldService worldService,
        ITimeManager timeManager)
        : base(position, healthPoints, energy, environment, timeManager)
    {
        _worldService = worldService;
        RootRadius = rootRadius;
        SeedRadius = seedRadius;
        ContactRadius = contactRadius;
    }

    protected override void UpdateBehavior()
    {
        ConsumeEnergy(SimulationConstants.BASE_ENERGY_LOSS * _timeManager.DeltaTime);
        
        var nearbyWaste = _worldService.GetEntitiesInRange(Position, RootRadius)
            .OfType<OrganicWaste>()
            .ToList();

        foreach (var waste in nearbyWaste)
        {
            AbsorbWaste(waste);
        }

        _growthAccumulator += _timeManager.DeltaTime;
        _reproductionAccumulator += _timeManager.DeltaTime;

        if (_growthAccumulator >= SimulationConstants.PLANT_GROWTH_INTERVAL)
        {
            ProcessGrowth();
            _growthAccumulator = 0;
        }

        if (_reproductionAccumulator >= SimulationConstants.PLANT_REPRODUCTION_INTERVAL)
        {
            if (CanSpreadSeeds())
            {
                SpreadSeeds();
            }
            _reproductionAccumulator = 0;
        }
    }

    protected virtual void ProcessGrowth()
    {
        if (Environment.HasFlag(PreferredEnvironment))
        {
            Energy += 1;
        }
    }

    protected virtual void AbsorbWaste(OrganicWaste waste)
    {
        double absorbedEnergy = waste.EnergyValue * BaseAbsorptionRate * _timeManager.DeltaTime;
        
        Console.WriteLine($"[{GetType().Name}#{TypeId}] processing waste absorption: Energy={absorbedEnergy:F3}, Threshold={SimulationConstants.WASTE_ABSORPTION_THRESHOLD:F3}");

        if (absorbedEnergy >= SimulationConstants.WASTE_ABSORPTION_THRESHOLD)
        {
            int energyToAbsorb = (int)Math.Ceiling(absorbedEnergy);
            Energy += energyToAbsorb;
            waste.EnergyValue -= energyToAbsorb;

            double growthFactor = energyToAbsorb * SimulationConstants.ROOT_GROWTH_RATE;
            RootRadius += growthFactor;
            RootRadius = Math.Min(RootRadius, SimulationConstants.MAX_ROOT_RADIUS);
            
            Console.WriteLine($"[{GetType().Name}#{TypeId}] absorbed {energyToAbsorb} energy from waste, root growth: +{growthFactor:F6}, new radius: {RootRadius:F3}");
        }
    }

    protected abstract bool CanSpreadSeeds();
    protected abstract Plant CreateOffspring(Position position);

    private void SpreadSeeds()
    {
        var (x, y) = RandomHelper.GetRandomPositionInRadius(Position.X, Position.Y, SeedRadius);
        var randomPosition = new Position(x, y);
        var offspring = CreateOffspring(randomPosition);
    }

    protected override void Die()
    {
        var waste = new OrganicWaste(Position, Energy, _worldService);
        _worldService.AddEntity(waste);
        _worldService.RemoveEntity(this);
    }
}
