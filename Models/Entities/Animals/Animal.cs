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
using ecosystem.Services.Simulation;
using ecosystem.Helpers;
using ecosystem.Models.Radius;
using ecosystem.Services.Factory;

namespace ecosystem.Models.Entities.Animals;

public abstract class Animal : MoveableEntity, IEnvironmentSensitive, IHasVisionRange
{
    private double _behaviorUpdateAccumulator;
    protected readonly IEntityLocator<Animal> _entityLocator;
    protected readonly IWorldService _worldService;
    private readonly List<IBehavior<Animal>> _behaviors;
    private readonly IEntityFactory _entityFactory;
    protected readonly List<EnvironmentPreference> _environmentPreferences = new();
    public IReadOnlyList<EnvironmentPreference> PreferredEnvironments => _environmentPreferences;
    public abstract EnvironmentType PreferredEnvironment { get; }
    private double _biteCooldown = 0;
    protected abstract double BaseBiteCooldownDuration { get; }
    private bool _hasDied = false;

    protected Animal(
        IEntityLocator<Animal> entityLocator,
        IWorldService worldService,
        ITimeManager timeManager,
        IEntityFactory entityFactory,
        Position position,
        int healthPoints,
        int energy,
        bool isMale,
        double visionRadius,
        double contactRadius,
        double basalMetabolicRate,
        EnvironmentType environment) 
        : base(position, healthPoints, energy, environment, basalMetabolicRate, timeManager)
    {
        _entityLocator = entityLocator;
        _worldService = worldService;
        _entityFactory = entityFactory;
        IsMale = isMale;
        VisionRadius = visionRadius;
        ContactRadius = contactRadius;
        _behaviors = new List<IBehavior<Animal>>();
        AddBaseBehaviors();
    }

    public bool IsMale { get; set; }
    public double VisionRadius { get; protected set; }
    public bool IsAdult { get; set; }
    public double ReproductionCooldown { get; set; }
    public double HungerThreshold { get; set; }
    public double ReproductionEnergyThreshold { get; set; }
    public double ReproductionEnergyCost { get; set; }
    public bool IsPregnant { get; set; }
    public IWorldService WorldService => _worldService;

    public void AddBehavior(IBehavior<Animal> behavior)
    {
        _behaviors.Add(behavior);
    }

    private void AddBaseBehaviors()
    {
        if (!IsMale)
        {
            AddBehavior(new PheromoneEmittingBehavior(_worldService));  // Priority 1
            AddBehavior(new BirthBehavior());                           // Priority 4
        }
        else
        {
            AddBehavior(new PheromoneAttractedBehavior(_worldService)); // Priority 2
        }
        
        AddBehavior(new RestBehavior());                                // Priority 0
    }

    protected override void UpdateBehavior()
    {
        _behaviorUpdateAccumulator += _timeManager.DeltaTime;

        if (_behaviorUpdateAccumulator >= SimulationConstants.BEHAVIOR_UPDATE_INTERVAL)
        {
            if (_biteCooldown > 0)
            {
                _biteCooldown -= _timeManager.DeltaTime;
            }

            var currentEnv = _worldService.GetEnvironmentAt(Position);
            var envPreference = GetBestEnvironmentPreference(currentEnv);
            
            if (envPreference.Type == EnvironmentType.None)
            {
                TakeDamage(SimulationConstants.ENVIRONMENT_DAMAGE_RATE);
            }
            
            var behavior = GetCurrentBehavior();
            if (behavior != null)
            {
                behavior.Execute(this);
            }

            _behaviorUpdateAccumulator = 0;
        }
    }

    protected bool CanBiteBasedOnCooldown()
    {
        return _biteCooldown <= 0;
    }

    protected void SetBiteCooldown()
    {
        _biteCooldown = BaseBiteCooldownDuration;
    }

    protected override void Die()
    {
        if (_hasDied) return;
        _hasDied = true;

        int meatCount = (int)Math.Floor(MaxHealth * 0.5 / 20.0);
        Console.WriteLine($"[{GetType().Name}#{TypeId}] died, creating {meatCount} meat pieces");
        
        CreateMeat(meatCount);
        _worldService.RemoveEntity(this);
    }

    private void CreateMeat(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var (x, y) = RandomHelper.GetRandomPositionInRadius(
                Position.X, 
                Position.Y, 
                ContactRadius * 2);
                
            x = Math.Clamp(x, 0, 1);
            y = Math.Clamp(y, 0, 1);
            
            var meatPosition = new Position(x, y);
            var meat = _entityFactory.CreateMeat(meatPosition);
            _worldService.AddEntity(meat);
        }
    }

    public EnvironmentPreference GetBestEnvironmentPreference(EnvironmentType currentEnv)
    {
        return PreferredEnvironments
            .Where(p => (p.Type & currentEnv) != 0)
            .OrderByDescending(p => p.MovementModifier)
            .FirstOrDefault() 
            ?? new EnvironmentPreference(EnvironmentType.None, 0.3, 3.0);
    }

    public abstract Animal CreateOffspring(Position position);

    protected override IBehavior<LifeForm>? GetCurrentBehavior()
    {
        var behavior = _behaviors
            .Where(b => 
            {
                var canExecute = b.CanExecute(this);
                return canExecute;
            })
            .OrderByDescending(b => b.Priority)
            .FirstOrDefault();

        if (behavior != null)
        {
            Stats.CurrentBehavior = behavior.Name;
            return new BehaviorWrapper<Animal, LifeForm>(behavior, this);
        }
        
        Stats.CurrentBehavior = "None";
        return null;
    }

    public void ConvertEnergyToHealth(double amount)
    {
        if (Energy >= SimulationConstants.HEALING_ENERGY_THRESHOLD &&
            HealthPoints < MaxHealth)
        {
            var excessEnergy = Math.Min(amount, Energy - SimulationConstants.HEALING_ENERGY_THRESHOLD);
            var healingAmount = (int)(excessEnergy * SimulationConstants.HEALING_CONVERSION_RATE);
            
            if (healingAmount > 0)
            {
                RemoveEnergy(healingAmount);
                Heal(healingAmount);
                Console.WriteLine($"[{GetType().Name}#{TypeId}] converted {healingAmount} energy to health");
            }
        }
    }

    public void Heal(int amount)
    {
        HealthPoints = Math.Min(MaxHealth, HealthPoints + amount);
    }
}
