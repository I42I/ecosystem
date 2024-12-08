using System;
using System.Collections.Generic;
using ecosystem.Models.Core;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Behaviors;

namespace ecosystem.Models.Entities.Animals;

public abstract class Animal : LifeForm, IMoveable, IReproducible
{
    private readonly IEntityLocator<Animal> _entityLocator;

    protected Animal(
        IEntityLocator<Animal> entityLocator,
        int healthPoints,
        int energy,
        (double X, double Y) position,
        bool isMale,
        double visionRadius,
        double contactRadius,
        double basalMetabolicRate,
        EnvironmentType environment) 
        : base(healthPoints, energy, position, basalMetabolicRate, environment)
    {
        _entityLocator = entityLocator;
        IsMale = isMale;
        VisionRadius = visionRadius;
        ContactRadius = contactRadius;
    }

    public bool IsMale { get; set; }
    public double VisionRadius { get; set; }
    public double ContactRadius { get; set; }
    public bool IsAdult { get; set; }
    public double ReproductionCooldown { get; set; }
    public double HungerThreshold { get; set; }
    public double ReproductionEnergyThreshold { get; set; }
    public double MovementSpeed { get; set; }
    public double ReproductionEnergyCost { get; set; }

    // Implémentation de IMoveable
    public void Move(double deltaX, double deltaY)
    {
        Position = (Position.X + deltaX, Position.Y + deltaY);
        InternalConsumeEnergy(CalculateMovementEnergyCost(deltaX, deltaY));
    }

    public double GetDistanceTo(IMoveable other)
    {
        double deltaX = Position.X - other.Position.X;
        double deltaY = Position.Y - other.Position.Y;
        return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }

    public override double GetEnvironmentMovementModifier()
    {
        return Environment.HasFlag(PreferredEnvironment) ? 1.0 : 1.5;
    }

    protected abstract int CalculateMovementEnergyCost(double deltaX, double deltaY);

    public bool CanReproduce()
    {
        return NeedsToReproduce();
    }

    public void Reproduce(IReproducible partner)
    {
        if (partner is Animal animalPartner)
        {
            Reproduce(animalPartner);
        }
    }

    protected void Reproduce(Animal partner)
    {
        if (IsMale != partner.IsMale && IsInContactWith(partner))
        {
            GiveBirth();
        }
    }

    protected abstract void GiveBirth();

    protected bool IsInContactWith(LifeForm other)
    {
        double distance = GetDistanceTo((IMoveable)other);
        return distance <= ContactRadius;
    }

    protected override void UpdateBehavior()
    {
        if (NeedsToEat())
        {
            SearchForFood();
        }
        else if (NeedsToReproduce())
        {
            SearchForMate();
        }
        else
        {
            Rest();
        }
    }

    protected abstract void SearchForFood();
    public abstract void SearchForMate();

    protected virtual void Rest()
    {
        //  vide ou réduire le métabolisme
    }

    protected bool NeedsToEat()
    {
        return Energy <= HungerThreshold;
    }

    protected bool NeedsToReproduce()
    {
        return IsAdult && ReproductionCooldown <= 0 && Energy >= ReproductionEnergyThreshold;
    }

    protected override void OnDeath()
    {
        CreateMeat();
    }

    private void CreateMeat()
    {
        // Instantiate a Meat object at the animal's position
        // Meat meat = new Meat(Position);
        // Add meat to the ecosystem
    }

    public override abstract EnvironmentType PreferredEnvironment { get; }

    protected Animal? FindNearestMate()
    {
        return _entityLocator.FindNearest(
            GetPotentialMates(),
            VisionRadius
        );
    }

    private IEnumerable<Animal> GetPotentialMates()
    {
        return new List<Animal>();
    }
}
