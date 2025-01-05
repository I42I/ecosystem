using System;
using Avalonia.Media;
using ecosystem.Models.Entities.Environment;
using ecosystem.Helpers;
using ecosystem.Models.Core;
using ecosystem.Services.World;
using ecosystem.Services.Simulation;
using ecosystem.Services.Factory;
using ecosystem.Models.Behaviors.Survival;
using ecosystem.Models.Behaviors.Movement;
using ecosystem.Models.Behaviors.Reproduction;
using ecosystem.Models.Behaviors.Base;
using ecosystem.Models.Entities.Plants;
using ecosystem.Models.Animation;
using System.Collections.Generic;

namespace ecosystem.Models.Entities.Animals.Herbivores;

public class Fish : Herbivore, IReproductionConstants
{
    public static int DefaultMaxHealth => 40;
    public static int DefaultMaxEnergy => 60;
    public override int MaxHealth => DefaultMaxHealth;
    public override int MaxEnergy => DefaultMaxEnergy;
    protected override int BaseBiteSize => 3;
    protected override double BaseBiteCooldownDuration => 0.02;
    public override double BaseHungerThreshold => 45.0;
    protected override double BaseReproductionThreshold => 55.0;
    protected override double BaseReproductionEnergyCost => 15.0;
    protected override double SpeciesEnergyCostModifier => 0.7;
    public static EnvironmentType DefaultEnvironment => EnvironmentType.Water;
    public override EnvironmentType PreferredEnvironment => DefaultEnvironment;
    
    public double GestationPeriod => 0.2;
    public double MaleReproductionCooldown => 8.0;
    public double FemaleReproductionCooldown => 12.0;
    
    private readonly IEntityFactory _entityFactory;

    public Fish(
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
            visionRadius: 0.08,
            contactRadius: 0.01,
            basalMetabolicRate: 0.7)
    {
        _entityFactory = entityFactory;
        MovementSpeed = 1.2;
        Color = Brushes.Silver;

        _environmentPreferences.Clear();
        _environmentPreferences.Add(new EnvironmentPreference(
            PreferredEnvironment, 
            movementMod: 1.0,
            energyLossMod: 1.0
        ));
        _environmentPreferences.Add(new EnvironmentPreference(
            EnvironmentType.Ground,
            movementMod: 0.0,
            energyLossMod: 5.0
        ));

        AddBehavior(new FleeingBehavior(_worldService));
        AddBehavior(new HungerBehavior());
        AddBehavior(new GroupMovementBehavior(_worldService));
        AddBehavior(new RestBehavior());

        try 
        {
            var spritePath = AssetHelper.GetAssetPath("Fish.png");
            InitializeSprite(spritePath, 48, 48);

            Console.WriteLine("Initializing Fish animations");

            Sprite?.AddAnimation(AnimationState.Dying, 
                new AnimatedSprite.AnimationConfig(
                    row: 0,
                    startFrame: 0,
                    frameCount: 6,
                    frameDuration: 0.02,
                    loop: false));

            Sprite?.AddAnimation(AnimationState.TakingDamage, 
                new AnimatedSprite.AnimationConfig(
                    row: 1,
                    startFrame: 0,    
                    frameCount: 2,
                    frameDuration: 0.02,
                    loop: false));

            Sprite?.AddAnimation(AnimationState.Idle, 
                new AnimatedSprite.AnimationConfig(
                    row: 2,
                    startFrame: 0,    
                    frameCount: 4,
                    frameDuration: 0.02));

            Sprite?.AddAnimation(AnimationState.Moving,
                new AnimatedSprite.AnimationConfig(
                    row: 3,
                    startFrame: 0,
                    frameCount: 4,
                    frameDuration: 0.02));

            Console.WriteLine("Fish animations initialized successfully");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load fish sprites: {ex.Message}");
        }
        
        Console.WriteLine($"Created Fish with color {Color} at {Position.X}, {Position.Y}");
    }

    public override Animal CreateOffspring(Position position)
    {
        return _entityFactory.CreateAnimal<Fish>(60, 80, position, RandomHelper.Instance.NextDouble() > 0.5);
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