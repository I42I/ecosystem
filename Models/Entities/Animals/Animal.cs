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
using ecosystem.Models.Animation;

namespace ecosystem.Models.Entities.Animals;

public abstract class Animal : MoveableEntity, IEnvironmentSensitive, IHasVisionRange, IAnimatable
{
    private double _behaviorUpdateAccumulator;
    protected readonly IEntityLocator<Animal> _entityLocator;
    private readonly List<IBehavior<Animal>> _behaviors;
    private readonly IEntityFactory _entityFactory;
    private readonly IDigestive _digestionSystem;
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
        : base(position, healthPoints, energy, environment, basalMetabolicRate, timeManager, worldService)
    {
        _entityLocator = entityLocator;
        _entityFactory = entityFactory;
        _digestionSystem = new DigestionSystem(this, _worldService, _timeManager);
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

            if (IsPregnant && _currentPregnancy.HasValue)
            {
                var pregnancy = _currentPregnancy.Value;
                pregnancy.GestationProgress += _timeManager.DeltaTime / 
                    ((this as IReproductionConstants)?.GestationPeriod ?? SimulationConstants.GESTATION_PERIOD);
                _currentPregnancy = pregnancy;

                if (pregnancy.GestationProgress >= 1.0)
                {
                    IsPregnant = false;
                    _currentPregnancy = null;
                }
            }

            _digestionSystem.ProcessDigestion(_timeManager.DeltaTime);

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

    public virtual int GetOffspringCount()
    {
        return 1;
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
        if (Energy >= SimulationConstants.HEALING_ENERGY_THRESHOLD/100 * MaxEnergy &&
            HealthPoints < MaxHealth)
        {
            var excessEnergy = Math.Min(amount, Energy - SimulationConstants.HEALING_ENERGY_THRESHOLD);
            var healingAmount = (int)(excessEnergy * SimulationConstants.HEALING_CONVERSION_RATE);
            
            if (healingAmount > 0)
            {
                RemoveEnergy(healingAmount);
                Heal(healingAmount);
            }
        }
    }

    public void Heal(int amount)
    {
        HealthPoints = Math.Min(MaxHealth, HealthPoints + amount);
    }

    protected struct Pregnancy
    {
        public double GestationProgress { get; set; }
        public Animal Father { get; init; }
    }
    
    private Pregnancy? _currentPregnancy;

    public void StartPregnancy(Animal father)
    {
        if (!IsPregnant && !IsMale)
        {
            IsPregnant = true;
            _currentPregnancy = new Pregnancy
            {
                GestationProgress = 0,
                Father = father
            };
            ReproductionCooldown = (this as IReproductionConstants)?.FemaleReproductionCooldown 
                ?? SimulationConstants.FEMALE_REPRODUCTION_COOLDOWN;
        }
    }

    public bool IsReadyToGiveBirth()
    {
        return _currentPregnancy?.GestationProgress >= 1.0;
    }

    protected void ProcessFoodConsumption(int amount)
    {
        _digestionSystem.AddFood(amount);
    }

    public virtual AnimatedSprite? Sprite { get; protected set; }
    public virtual AnimationState CurrentState { get; protected set; }

    protected virtual AnimationState DetermineAnimationState()
    {
        if (IsDead) return AnimationState.Dead;
        return MovementSpeed > 0 ? AnimationState.Moving : AnimationState.Idle;
    }

    protected IAnimationManager? _animationManager;
    
    protected virtual void InitializeSprite(string spritePath, int frameWidth, int frameHeight)
    {
        Sprite = new AnimatedSprite(spritePath, frameWidth, frameHeight);
        _animationManager = new AnimationManager(Sprite);
    }

    public virtual void UpdateAnimation(double deltaTime)
    {
        if (_animationManager == null) return;
        
        if (!_animationManager.HasQueuedAnimations)
        {
            bool isMoving = Math.Abs(_currentDirectionX) > 0.01 || 
                        Math.Abs(_currentDirectionY) > 0.01;
                        
            if (isMoving && _animationManager.CurrentState != AnimationState.Moving)
            {
                _animationManager.PlayAnimation(new AnimationEvent(AnimationState.Moving));
            }
        }
        
        _animationManager.Update(deltaTime);
    }
}
