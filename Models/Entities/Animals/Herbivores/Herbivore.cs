using System;
using System.Collections.Generic;
using ecosystem.Models;
using ecosystem.Models.Behaviors;
using ecosystem.Helpers;
using System.Linq;
using ecosystem.Models.Entities.Plants;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Core;
using ecosystem.Services.World;
using ecosystem.Services.Simulation;

namespace ecosystem.Models.Entities.Animals.Herbivores;

public abstract class Herbivore : Animal
{
    protected readonly IEntityLocator<Plant> _plantLocator;
    protected abstract double BaseHungerThreshold { get; }
    protected abstract double BaseReproductionThreshold { get; }
    protected abstract double BaseReproductionEnergyCost { get; }
    protected abstract int BaseBiteSize { get; }

    protected Herbivore(
        IEntityLocator<Animal> entityLocator,
        IEntityLocator<Plant> plantLocator,
        IWorldService worldService,
        ITimeManager timeManager,
        Position position,
        int healthPoints,
        int energy,
        bool isMale,
        double visionRadius,
        double contactRadius,
        double basalMetabolicRate)
        : base(entityLocator, worldService, timeManager, position, healthPoints, energy, isMale,
               visionRadius, contactRadius, basalMetabolicRate, EnvironmentType.Ground)
    {
        _plantLocator = plantLocator;
    }

    protected Plant? FindNearestPlant()
    {
        return _plantLocator.FindNearest(
            _worldService.Entities.OfType<Plant>(),
            VisionRadius
        );
    }

    protected virtual void Eat(Plant plant)
    {
        if (!plant.IsDead)
        {
            int energyGained = BaseBiteSize / 2;
            plant.TakeDamage(BaseBiteSize);
            Energy += energyGained;
        }
    }
}