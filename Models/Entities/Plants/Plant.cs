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
    protected bool _hasCreatedWaste = false;
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
        if (_growthAccumulator >= SimulationConstants.PLANT_GROWTH_INTERVAL)
        {
            ProcessGrowth();
            _growthAccumulator = 0;
        }

        _reproductionAccumulator += _timeManager.DeltaTime;
        if (_reproductionAccumulator >= SimulationConstants.PLANT_REPRODUCTION_CHECK_INTERVAL)
        {
            double healthFactor = (double)HealthPoints / MaxHealth;
            double energyFactor = (double)Energy / MaxEnergy;
            double reproductionChance = SimulationConstants.PLANT_SEED_SPREAD_BASE_CHANCE * 
                                    healthFactor * 
                                    energyFactor;

            if (CanSpreadSeeds() && RandomHelper.Instance.NextDouble() < reproductionChance)
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

        if (absorbedEnergy >= SimulationConstants.WASTE_ABSORPTION_THRESHOLD)
        {
            int energyToAbsorb = (int)Math.Ceiling(absorbedEnergy);
            Energy += energyToAbsorb;
            waste.EnergyValue -= energyToAbsorb;

            double growthFactor = energyToAbsorb * SimulationConstants.ROOT_GROWTH_RATE;
            RootRadius += growthFactor;
            RootRadius = Math.Min(RootRadius, SimulationConstants.MAX_ROOT_RADIUS);
        }
    }

    protected virtual bool CanSpreadSeeds()
    {
        return Energy >= SimulationConstants.PLANT_MIN_ENERGY_FOR_REPRODUCTION && 
            HealthPoints >= SimulationConstants.PLANT_MIN_HEALTH_FOR_REPRODUCTION;
    }

    protected abstract Plant CreateOffspring(Position position);

    private void SpreadSeeds()
    {
        var (x, y) = RandomHelper.GetRandomPositionInRadius(
            Position.X,
            Position.Y,
            SeedRadius
        );
        
        var newPosition = new Position(x, y);
        
        if (_worldService.GetEnvironmentAt(newPosition).HasFlag(PreferredEnvironment))
        {
            var offspring = CreateOffspring(newPosition);
            Energy -= (int)SimulationConstants.PLANT_REPRODUCTION_ENERGY_COST;
            
            _worldService.AddEntity(offspring);
            Console.WriteLine($"{GetType().Name}#{TypeId} spread seeds at ({x:F2}, {y:F2})");
        }
    }

    protected override void Die()
    {
        if (_hasCreatedWaste) return;
        _hasCreatedWaste = true;

        var waste = new OrganicWaste(Position, Energy, _worldService);
        _worldService.AddEntity(waste);
        _worldService.RemoveEntity(this);
    }
}
