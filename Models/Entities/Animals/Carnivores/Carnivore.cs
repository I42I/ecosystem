using System;
using System.Collections.Generic;
using ecosystem.Models;
using ecosystem.Helpers;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Behaviors;
using ecosystem.Models.Behaviors.Hunt;
using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Core;
using ecosystem.Services.World;
using System.Linq;
using ecosystem.Models.Entities.Animals.Herbivores;
using ecosystem.Services.Simulation;
using ecosystem.Services.Factory;

namespace ecosystem.Models.Entities.Animals.Carnivores;

public abstract class Carnivore : Animal
{
    protected readonly IEntityLocator<Animal> _preyLocator;
    public abstract double BaseAttackPower { get; }
    protected abstract double BaseAttackRange { get; }
    public abstract double BaseHungerThreshold { get; }
    protected abstract double BaseReproductionThreshold { get; }
    protected abstract double BaseReproductionEnergyCost { get; }
    protected double AttackPower { get; set; }
    protected double AttackRange { get; set; }

    protected Carnivore(
        IEntityLocator<Animal> entityLocator,
        IEntityLocator<Animal> preyLocator,
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
        _preyLocator = preyLocator ?? throw new ArgumentNullException(nameof(preyLocator));
        AttackPower = BaseAttackPower;
        AttackRange = BaseAttackRange;
        HungerThreshold = BaseHungerThreshold;
        ReproductionEnergyThreshold = BaseReproductionThreshold;
    }

    public virtual bool CanAttack(Animal prey)
    {
        return MathHelper.IsInContactWith(this, prey);
    }

    public virtual void Attack(Animal prey)
    {
        if (CanAttack(prey) && CanBiteBasedOnCooldown())
        {
            int damage = CalculateAttackDamage();
            prey.TakeDamage(damage);
            
            int energyGained = damage;
            energyGained = Math.Min(energyGained, MaxEnergy - Energy);
            
            if (energyGained > 0)
            {
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
                    }
                }
            }
        }
    }

    protected virtual int CalculateAttackDamage()
    {
        return (int)(BaseAttackPower * (0.8 + RandomHelper.Instance.NextDouble() * 0.4));
    }

    public virtual void Eat(Meat meat)
    {
        if (meat.IsDead || !CanBiteBasedOnCooldown()) return;

        int damageDealt = CalculateAttackDamage();
        meat.TakeDamage(damageDealt);
        
        int energyGained = damageDealt * 4;
        energyGained = Math.Min(energyGained, MaxEnergy - Energy);
        
        if (energyGained > 0)
        {
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
                }
            }
        }
    }
}
