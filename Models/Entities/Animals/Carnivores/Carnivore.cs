using System;
using System.Collections.Generic;
using ecosystem.Models;
using ecosystem.Helpers;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Behaviors;
using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Core;
using ecosystem.Services.World;

namespace ecosystem.Models.Entities.Animals.Carnivores;

public abstract class Carnivore : Animal, IPredator
{
    private readonly IEntityLocator<Animal> _preyLocator;

    protected abstract double BaseAttackPower { get; }
    protected abstract double BaseAttackRange { get; }
    protected abstract double BaseHungerThreshold { get; }
    protected abstract double BaseReproductionThreshold { get; }
    protected abstract double BaseReproductionEnergyCost { get; }

    protected Carnivore(
        IEntityLocator<Animal> entityLocator,
        IEntityLocator<Animal> preyLocator,
        IWorldService worldService, 
        Position position,
        int healthPoints,
        int energy,
        bool isMale,
        double visionRadius,
        double contactRadius,
        double basalMetabolicRate)
        : base(entityLocator, worldService, position, healthPoints, energy, isMale, 
               visionRadius, contactRadius, basalMetabolicRate, EnvironmentType.Ground)
    {
        _preyLocator = preyLocator;
        AttackPower = BaseAttackPower;
        AttackRange = BaseAttackRange;
        HungerThreshold = BaseHungerThreshold;
        ReproductionEnergyThreshold = BaseReproductionThreshold;
    }

    public double AttackPower { get; protected set; }
    public double AttackRange { get; protected set; }

    public virtual Animal? FindNearestPrey()
    {
        return _preyLocator.FindNearest(
            GetPotentialPrey(),
            VisionRadius
        );
    }

    protected abstract IEnumerable<Animal> GetPotentialPrey();

    public virtual void MoveTowardsPrey(Animal prey)
    {
        double distance = GetDistanceTo(prey);
        if (distance > 0)
        {
            double dx = (prey.Position.X - Position.X) / distance;
            double dy = (prey.Position.Y - Position.Y) / distance;
            Move(dx, dy);
        }
    }

    public virtual bool CanAttack(Animal prey)
    {
        return IsInContactWith(prey);
    }

    public virtual void Attack(Animal prey)
    {
        if (CanAttack(prey))
        {
            prey.TakeDamage(CalculateAttackDamage());
            Energy += CalculateEnergyGain();
        }
    }

    protected abstract int CalculateAttackDamage();
    protected abstract int CalculateEnergyGain();

    protected override void SearchForFood()
    {
        var prey = FindNearestPrey();
        if (prey != null)
        {
            MoveTowardsPrey(prey);
            if (CanAttack(prey))
            {
                Attack(prey);
            }
        }
        else
        {
            Rest();
        }
    }

    protected override int CalculateMovementEnergyCost(double deltaX, double deltaY)
    {
        double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        return (int)(distance * GetEnvironmentMovementModifier() * 1.2);
    }
}
