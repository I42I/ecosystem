using System;
using System.Collections.Generic;
using ecosystem.Models.Core;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Behaviors;
using ecosystem.Services.World;
using ecosystem.Helpers;

namespace ecosystem.Models.Entities.Animals;

public abstract class Animal : MoveableEntity, IReproducible
{
    protected readonly IEntityLocator<Animal> _entityLocator;
    protected readonly IWorldService _worldService;

    protected Animal(
        IEntityLocator<Animal> entityLocator,
        IWorldService worldService,
        Position position,
        int healthPoints,
        int energy,
        bool isMale,
        double visionRadius,
        double contactRadius,
        double basalMetabolicRate,
        EnvironmentType environment) 
        : base(healthPoints, energy, position, basalMetabolicRate, environment)
    {
        _entityLocator = entityLocator;
        _worldService = worldService;
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
    public double ReproductionEnergyCost { get; set; }

    public override double GetEnvironmentMovementModifier()
    {
        return Environment.HasFlag(PreferredEnvironment) ? 1.0 : 1.5;
    }

    protected override abstract int CalculateMovementEnergyCost(double deltaX, double deltaY);
    
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
        Console.WriteLine($"Updating behavior for {GetType().Name}");
        if (NeedsToEat())
        {
            Console.WriteLine("Needs to eat - searching for food");
            SearchForFood();
        }
        else if (NeedsToReproduce())
        {
            Console.WriteLine("Needs to reproduce - searching for mate");
            SearchForMate();
        }
        else
        {
            Console.WriteLine("Resting");
            Rest();
        }
    }

    protected abstract void SearchForFood();
    public abstract void SearchForMate();

    protected virtual void Rest()
    {
        double angle = RandomHelper.Instance.NextDouble() * 2 * Math.PI;
        double dx = Math.Cos(angle);
        double dy = Math.Sin(angle);
        Move(dx, dy);
    }

    protected bool NeedsToEat()
    {
        Console.WriteLine($"Energy: {Energy}, HungerThreshold: {HungerThreshold}, HealthPoints: {HealthPoints}");
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
