using System;
using System.Collections.Generic;
using System.Linq;
using ecosystem.Models.Core;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Behaviors;
using ecosystem.Models.Behaviors.Base;
using ecosystem.Models.Behaviors.Survival;
using ecosystem.Models.Behaviors.Reproduction;
using ecosystem.Models.Behaviors.Movement;
using ecosystem.Services.World;
using ecosystem.Helpers;

namespace ecosystem.Models.Entities.Animals;

public abstract class Animal : MoveableEntity, IReproducible
{
    protected readonly IEntityLocator<Animal> _entityLocator;
    protected readonly IWorldService _worldService;
    private readonly List<IBehavior<Animal>> _behaviors;

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
        _behaviors = new List<IBehavior<Animal>>();
    }

    public bool IsMale { get; set; }
    public double VisionRadius { get; set; }
    public double ContactRadius { get; set; }
    public bool IsAdult { get; set; }
    public double ReproductionCooldown { get; set; }
    public double HungerThreshold { get; set; }
    public double ReproductionEnergyThreshold { get; set; }
    public double ReproductionEnergyCost { get; set; }
    public bool IsPregnant { get; set; }
    protected IWorldService WorldService => _worldService;

    public override double GetEnvironmentMovementModifier()
    {
        return Environment.HasFlag(PreferredEnvironment) ? 1.0 : 2.0;
    }

    // protected override abstract int CalculateMovementEnergyCost(double deltaX, double deltaY);

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

    protected bool IsInContactWith(LifeForm other)
    {
        double distance = GetDistanceTo(other.Position);
        return distance <= ContactRadius;
    }

    public void AddBehavior(IBehavior<Animal> behavior)
    {
        _behaviors.Add(behavior);
    }

    protected override void UpdateBehavior()
    {
        var applicableBehavior = _behaviors
            .Where(b => b.CanExecute(this))
            .OrderByDescending(b => b.Priority)
            .FirstOrDefault();

        applicableBehavior?.Execute(this);
    }

    public abstract void SearchForFood();
    
    public virtual void Rest()
    {
        Console.WriteLine($"{GetType().Name} is resting");
        if (_directionChangeTicks <= 0)
        {
            double angle = RandomHelper.Instance.NextDouble() * 2 * Math.PI;
            _currentDirectionX = Math.Cos(angle);
            _currentDirectionY = Math.Sin(angle);
            _directionChangeTicks = RandomHelper.Instance.Next(60, 180);
        }
        
        _directionChangeTicks--;
        
        double variation = (RandomHelper.Instance.NextDouble() - 0.5) * 0.2;
        double dx = _currentDirectionX + variation;
        double dy = _currentDirectionY + variation;

        double length = Math.Sqrt(dx * dx + dy * dy);
        if (length > 0)
        {
            dx /= length;
            dy /= length;
        }
        
        Move(dx, dy);
    }

    protected bool NeedsToEat()
    {
        Console.WriteLine($"{GetType().Name} - Energy: {Energy}, HungerThreshold: {HungerThreshold}, HealthPoints: {HealthPoints}");
        return Energy <= HungerThreshold;
    }

    public bool NeedsToReproduce()
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

    protected abstract Animal CreateOffspring(Position position);
}
