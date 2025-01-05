using System;
using Avalonia.Media;
using System.Linq;
using ecosystem.Models.Entities.Environment;
using ecosystem.Helpers;
using ecosystem.Models.Behaviors;
using ecosystem.Models.Entities.Plants;
using ecosystem.Services.World;
using ecosystem.Models.Core;
using ecosystem.Models.Behaviors.Survival;
using ecosystem.Models.Behaviors.Movement;
using ecosystem.Models.Behaviors.Reproduction;
using ecosystem.Models.Behaviors.Base;
using ecosystem.Services.Simulation;
using ecosystem.Services.Factory;
using ecosystem.Models.Animation;

namespace ecosystem.Models.Entities.Animals.Herbivores;

public class Duck : Herbivore, IReproductionConstants
{
    public static int DefaultMaxHealth => 70;
    public static int DefaultMaxEnergy => 90;
    public override int MaxHealth => DefaultMaxHealth;
    public override int MaxEnergy => DefaultMaxEnergy;
    protected override int BaseBiteSize => 4;
    protected override double BaseBiteCooldownDuration => 0.03;
    public override double BaseHungerThreshold => 50.0;
    protected override double BaseReproductionThreshold => 65.0;
    protected override double BaseReproductionEnergyCost => 25.0;
    protected override double SpeciesEnergyCostModifier => 0.8;
    public static EnvironmentType DefaultEnvironment => EnvironmentType.Ground | EnvironmentType.Water;
    public override EnvironmentType PreferredEnvironment => DefaultEnvironment;

    public double GestationPeriod => 0.3;
    public double MaleReproductionCooldown => 10.0;
    public double FemaleReproductionCooldown => 15.0;

    private readonly IEntityFactory _entityFactory;

    public Duck(
        IEntityLocator<Animal> entityLocator,
        IEntityLocator<Plant> plantLocator,
        IWorldService worldService,
        ITimeManager timeManager,
        IEntityFactory entityFactory,
        Position position,
        int healthPoints,
        int energy,
        bool isMale)
        : base(
            entityLocator,
            plantLocator,
            worldService,
            timeManager,
            entityFactory,
            position,
            healthPoints,
            energy,
            isMale,
            visionRadius: 0.15,
            contactRadius: 0.01,
            basalMetabolicRate: 0.7)
    {
        _entityFactory = entityFactory;
        MovementSpeed = 1.3;
        Color = Brushes.Yellow;

        _environmentPreferences.Clear();
        _environmentPreferences.Add(new EnvironmentPreference(
            EnvironmentType.Ground,
            movementMod: 1.0,
            energyLossMod: 1.0
        ));
        _environmentPreferences.Add(new EnvironmentPreference(
            EnvironmentType.Water,
            movementMod: 1.2,
            energyLossMod: 0.8
        ));

        AddBehavior(new FleeingBehavior(_worldService));
        AddBehavior(new HungerBehavior());
        AddBehavior(new GroupMovementBehavior(_worldService));
        AddBehavior(new RestBehavior());

        try 
        {
            var spritePath = AssetHelper.GetAssetPath("Duck.png");
            InitializeSprite(spritePath, 32, 32);

            Console.WriteLine("Initializing Duck animations");

            Sprite?.AddAnimation(AnimationState.Idle, 
                new AnimatedSprite.AnimationConfig(
                    row: 0,
                    startFrame: 0,
                    frameCount: 2,
                    frameDuration: 0.02));

            Sprite?.AddAnimation(AnimationState.Moving, 
                new AnimatedSprite.AnimationConfig(
                    row: 1,
                    startFrame: 0,    
                    frameCount: 6,
                    frameDuration: 0.02));

            Sprite?.AddAnimation(AnimationState.Eat, 
                new AnimatedSprite.AnimationConfig(
                    row: 2,
                    startFrame: 0,    
                    frameCount: 4,
                    frameDuration: 0.01,
                    loop: false));

            Sprite?.AddAnimation(AnimationState.TakingDamage,
                new AnimatedSprite.AnimationConfig(
                    row: 3,
                    startFrame: 0,
                    frameCount: 6,
                    frameDuration: 0.02,
                    loop: false));

            Console.WriteLine("Duck animations initialized successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load duck sprite: {ex.Message}");
        }

        Console.WriteLine($"Created Duck with color {Color} at {Position.X}, {Position.Y}");
    }

    public override Animal CreateOffspring(Position position)
    {
        return _entityFactory.CreateAnimal<Duck>(60, 80, position, RandomHelper.Instance.NextDouble() > 0.5);
    }

    public override void Eat(Plant plant)
    {
        base.Eat(plant);
        _animationManager?.PlayAnimation(new AnimationEvent(AnimationState.Eat, true)); 
    }

    public override void TakeDamage(double amount)
    {
        base.TakeDamage(amount);
        _animationManager?.PlayAnimation(new AnimationEvent(AnimationState.TakingDamage, true));
    }

    public override void UpdateAnimation(double deltaTime)
    {
        if (_animationManager == null) return;

        if (!_animationManager.HasQueuedAnimations)
        {
            AnimationState targetState = IsMoving ? AnimationState.Moving : AnimationState.Idle;

            if (_animationManager.CurrentState != targetState)
            {
                _animationManager.PlayAnimation(new AnimationEvent(targetState));
            }
        }
        
        _animationManager.Update(deltaTime);
    }
}