using System;
using System.Linq;
using System.Collections.Generic;
using ecosystem.Helpers;
using ecosystem.Models.Core;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Behaviors;
using ecosystem.Services.World;

namespace ecosystem.Models.Entities.Plants;

public abstract class Plant : LifeForm
{
    protected abstract double BaseAbsorptionRate { get; }
    protected readonly IWorldService _worldService;

    protected Plant(
        int healthPoints,
        int energy,
        Position position,
        double basalMetabolicRate,
        EnvironmentType environment,
        IWorldService worldService)
        : base(healthPoints, energy, position, basalMetabolicRate, environment)
    {
        _worldService = worldService;
    }

    public double RootRadius { get; protected set; }
    public double SeedRadius { get; protected set; }

    protected override void UpdateBehavior()
    {
        ConsumeEnergy(1);

        var nearbyWaste = _worldService.Entities
            .OfType<OrganicWaste>()
            .Where(w => GetDistanceTo(w.Position) <= RootRadius)
            .ToList();

        foreach (var waste in nearbyWaste)
        {
            AbsorbWaste(waste);
        }

        if (CanSpreadSeeds())
        {
            SpreadSeeds();
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

    protected override void OnDeath()
    {
        var waste = new OrganicWaste(Position, Energy);
        _worldService.AddEntity(waste);
    }
}
