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
    public abstract double BaseHungerThreshold { get; }
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

    public Plant? FindNearestPlant()
    {
        var plants = _worldService.Entities.OfType<Plant>().ToList();
        Console.WriteLine($"[{GetType().Name}#{TypeId}] Found {plants.Count} plants in world");
        
        foreach (var plant in plants)
        {
            var distance = GetDistanceTo(plant.Position);
            Console.WriteLine($"[{GetType().Name}#{TypeId}] Distance to plant: {distance:F3}, Vision radius: {VisionRadius:F3}");
        }
        
        var nearestPlant = _plantLocator.FindNearest(plants, VisionRadius, Position);
        
        if (nearestPlant != null)
        {
            var distance = GetDistanceTo(nearestPlant.Position);
            Console.WriteLine($"[{GetType().Name}#{TypeId}] Found nearest plant at distance {distance:F3}");
        }
        
        return nearestPlant;
    }

    public virtual void Eat(Plant plant)
    {
        if (!plant.IsDead && CanBiteBasedOnCooldown())
        {
            int damageDealt = BaseBiteSize;
            plant.TakeDamage(damageDealt);
            
            int energyGained = (int)(damageDealt * 5);
            Energy += energyGained;
            
            SetBiteCooldown();
            
            Console.WriteLine($"[{GetType().Name}#{TypeId}] bit {plant.GetType().Name} for {damageDealt} damage, gained {energyGained} energy");
        }
    }
}