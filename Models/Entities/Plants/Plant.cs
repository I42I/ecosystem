using System;
using System.Linq;
using System.Collections.Generic;
using ecosystem.Helpers;
using ecosystem.Models.Core;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Behaviors;
using ecosystem.Services.World;
using ecosystem.Services.Simulation;

namespace ecosystem.Models.Entities.Plants;

public abstract class Plant : LifeForm
{
    private double _growthAccumulator;
    private double _reproductionAccumulator;
    protected abstract double BaseAbsorptionRate { get; }
    protected readonly IWorldService _worldService;
    public abstract EnvironmentType PreferredEnvironment { get; }

    protected Plant(
        int healthPoints,
        int energy,
        Position position,
        double basalMetabolicRate,
        EnvironmentType environment,
        IWorldService worldService,
        ITimeManager timeManager)
        : base(position, healthPoints, energy, environment, timeManager)
    {
        _worldService = worldService;
    }

    public double RootRadius { get; protected set; }
    public double SeedRadius { get; protected set; }

    protected override void UpdateBehavior()
    {
        ConsumeEnergy(SimulationConstants.BASE_ENERGY_LOSS * _timeManager.DeltaTime);

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
        double absorbedEnergy = waste.EnergyValue * BaseAbsorptionRate;
        Energy += (int)absorbedEnergy;
        waste.EnergyValue -= (int)absorbedEnergy;
        
        if (waste.EnergyValue <= 0)
        {
            _worldService.RemoveEntity(waste);
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
        var waste = new OrganicWaste(Position, Energy);
        _worldService.AddEntity(waste);
    }
}
