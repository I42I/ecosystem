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
    private double _biteCooldown = 0;
    protected abstract double BiteCooldownDuration { get; }
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

    public Plant? FindNearestPlant()
    {
        return _plantLocator.FindNearest(
            _worldService.Entities.OfType<Plant>(),
            VisionRadius
        );
    }

    protected override void UpdateBehavior()
    {
        base.UpdateBehavior();
        
        if (_biteCooldown > 0)
        {
            _biteCooldown -= _timeManager.DeltaTime;
        }
    }

    public virtual void Eat(Plant plant)
    {
        if (!plant.IsDead && _biteCooldown <= 0)
        {
            int damageDealt = BaseBiteSize;
            plant.TakeDamage(damageDealt);
            
            int energyGained = (int)(damageDealt * 0.5);
            Energy += energyGained;
            
            _biteCooldown = BiteCooldownDuration;
            
            Console.WriteLine($"{GetType().Name} bit {plant.GetType().Name} for {damageDealt} damage, gained {energyGained} energy");
        }
    }
}