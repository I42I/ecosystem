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

namespace ecosystem.Models.Entities.Animals.Carnivores;

public abstract class Carnivore : Animal, IPredator
{
    protected readonly IEntityLocator<Animal> _preyLocator;
    public abstract double BaseAttackPower { get; }
    protected abstract double BaseAttackRange { get; }
    protected abstract double BaseHungerThreshold { get; }
    protected abstract double BaseReproductionThreshold { get; }
    protected abstract double BaseReproductionEnergyCost { get; }
    protected double AttackPower { get; set; }
    protected double AttackRange { get; set; }

    protected Carnivore(
        IEntityLocator<Animal> entityLocator,
        IEntityLocator<Animal> preyLocator,
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
        _preyLocator = preyLocator ?? throw new ArgumentNullException(nameof(preyLocator));
        AttackPower = BaseAttackPower;
        AttackRange = BaseAttackRange;
        HungerThreshold = BaseHungerThreshold;
        ReproductionEnergyThreshold = BaseReproductionThreshold;
    }

    public virtual Animal? FindNearestPrey()
    {
        return _preyLocator.FindNearest(
            GetPotentialPrey(),
            VisionRadius
        );
    }

    public virtual void MoveTowardsPrey(Animal prey)
    {
        _directionChangeTicks = 0;
        
        double dx = prey.Position.X - Position.X;
        double dy = prey.Position.Y - Position.Y;
        double distance = Math.Sqrt(dx * dx + dy * dy);

        if (distance > 0)
        {
            _currentDirectionX = dx / distance;
            _currentDirectionY = dy / distance;
            Move(_currentDirectionX, _currentDirectionY);
        }
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
            Energy += damage / 4;

            SetBiteCooldown();
            
            if (prey.IsDead)
            {
                var meat = new Meat(prey.Position, prey.Energy, _timeManager);
                _worldService.AddEntity(meat);
            }
        }
    }

    protected virtual int CalculateAttackDamage()
    {
        // Comportement de base pour tous les carnivores
        return (int)(BaseAttackPower * (0.8 + RandomHelper.Instance.NextDouble() * 0.4));
    }

    protected virtual int CalculateEnergyGain()
    {
        // Énergie gagnée proportionnelle à l'attaque pour tous les carnivores
        return (int)(BaseAttackPower * 0.5);
    }

    protected virtual IEnumerable<Animal> GetPotentialPrey()
    {
        // Par défaut, tous les herbivores sont des proies potentielles
        return _worldService.Entities
            .OfType<Herbivore>()
            .Where(h => !h.IsDead);
    }
}
