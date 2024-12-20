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

namespace ecosystem.Models.Entities.Animals;

public abstract class Animal : MoveableEntity, IEnvironmentSensitive
{
    private double _behaviorUpdateAccumulator;
    protected readonly IEntityLocator<Animal> _entityLocator;
    protected readonly IWorldService _worldService;
    private readonly List<IBehavior<Animal>> _behaviors;
    protected readonly List<EnvironmentPreference> _environmentPreferences = new();
    public IReadOnlyList<EnvironmentPreference> PreferredEnvironments => _environmentPreferences;
    public abstract EnvironmentType PreferredEnvironment { get; }

    protected Animal(
        IEntityLocator<Animal> entityLocator,
        IWorldService worldService,
        ITimeManager timeManager,
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
        IsMale = isMale;
        VisionRadius = visionRadius;
        ContactRadius = contactRadius;
        _behaviors = new List<IBehavior<Animal>>();
        AddBaseBehaviors();
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
    public IWorldService WorldService => _worldService;

    public void AddBehavior(IBehavior<Animal> behavior)
    {
        _behaviors.Add(behavior);
    }

    private void AddBaseBehaviors()
    {
        AddBehavior(new MatingBehavior(_worldService, _timeManager));   // Priority 2
        AddBehavior(new RestBehavior());                                // Priority 0
    }

    protected override void UpdateBehavior()
    {
        _behaviorUpdateAccumulator += _timeManager.DeltaTime;

        if (_behaviorUpdateAccumulator >= SimulationConstants.BEHAVIOR_UPDATE_INTERVAL)
        {
            Console.WriteLine($"{GetType().Name} updating behavior !!!!!!!!!!!!!");

            var currentEnv = _worldService.GetEnvironmentAt(Position);
            var envPreference = GetBestEnvironmentPreference(currentEnv);
            
            if (envPreference.Type == EnvironmentType.None)
            {
                TakeDamage(SimulationConstants.ENVIRONMENT_DAMAGE_RATE);
            }
            
            var behavior = GetCurrentBehavior();
            Console.WriteLine($"Current behavior: {behavior}");
            if (behavior != null)
            {
                Console.WriteLine($"Executing behavior: {behavior.Name} !!!!!!!!!!!!!!!!!!!");
                behavior.Execute(this);
            }

            _behaviorUpdateAccumulator = 0;
        }
    }

    protected override void Die()
    {
        CreateMeat();
    }

    private void CreateMeat()
    {
        // Instantiate a Meat object at the animal's position
        // Meat meat = new Meat(Position);
        // Add meat to the ecosystem
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
                Console.WriteLine($"Behavior {b.Name} can execute: {canExecute}");
                return canExecute;
            })
            .OrderByDescending(b => b.Priority)
            .FirstOrDefault();

        if (behavior != null)
        {
            Stats.CurrentBehavior = behavior.Name;
            Console.WriteLine($"Selected behavior: {behavior}");
            return behavior as IBehavior<LifeForm>;
        }
        else
        {
            Stats.CurrentBehavior = "None";
            Console.WriteLine($"No behavior available for {GetType().Name}");
            return null;
        }
    }
}
