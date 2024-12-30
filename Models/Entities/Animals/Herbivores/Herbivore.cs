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
using ecosystem.Services.Factory;

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
        IEntityFactory entityFactory,
        Position position,
        int healthPoints,
        int energy,
        bool isMale,
        double visionRadius,
        double contactRadius,
        double basalMetabolicRate)
        : base(entityLocator, worldService, timeManager, entityFactory, position, healthPoints, energy, isMale,
               visionRadius, contactRadius, basalMetabolicRate, EnvironmentType.Ground)
    {
        _plantLocator = plantLocator;
    }

    public Plant? FindNearestPlant()
    {
        var plants = _worldService.Entities.OfType<Plant>().ToList();
        
        foreach (var plant in plants)
        {
            var distance = GetDistanceTo(plant.Position);
        }
        
        var nearestPlant = _plantLocator.FindNearest(plants, VisionRadius, Position);
        
        if (nearestPlant != null)
        {
            var distance = GetDistanceTo(nearestPlant.Position);
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
            energyGained = Math.Min(energyGained, MaxEnergy - Energy);
            
            Energy += energyGained;
            SetBiteCooldown();
            
            if (Energy >= SimulationConstants.HEALING_ENERGY_THRESHOLD && 
                HealthPoints < MaxHealth)
            {
                var excessEnergy = Energy - SimulationConstants.HEALING_ENERGY_THRESHOLD;
                var healingAmount = (int)(excessEnergy * SimulationConstants.HEALING_CONVERSION_RATE);
                
                if (healingAmount > 0)
                {
                    Energy -= healingAmount;
                    HealthPoints = Math.Min(MaxHealth, HealthPoints + healingAmount);
                    Console.WriteLine($"[{GetType().Name}#{TypeId}] healed for {healingAmount} HP using excess energy");
                }
            }
        }
    }
}