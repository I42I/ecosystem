using System;
using Avalonia.Media;
using ecosystem.Helpers;
using ecosystem.Models.Behaviors;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Behaviors.Hunt;
using ecosystem.Services.World;
using ecosystem.Models.Core;
using ecosystem.Models.Behaviors.Movement;
using ecosystem.Services.Simulation;
using ecosystem.Services.Factory;
using ecosystem.Models.Animation;

namespace ecosystem.Models.Entities.Animals.Carnivores;

public class Fox : Carnivore
{
    public static int DefaultMaxHealth => 100;
    public static int DefaultMaxEnergy => 100;
    public override int MaxHealth => DefaultMaxHealth;
    public override int MaxEnergy => DefaultMaxEnergy;
    public override double BaseAttackPower => 15.0;
    protected override double BaseAttackRange => 1.5;
    protected override double BaseBiteCooldownDuration => 0.03;
    public override double BaseHungerThreshold => 40.0;
    protected override double BaseReproductionThreshold => 60.0;
    protected override double BaseReproductionEnergyCost => 30.0;
    protected override double SpeciesEnergyCostModifier => 1.2;
    public override EnvironmentType PreferredEnvironment => EnvironmentType.Ground;
    private readonly Position _territoryCenter;
    private readonly IEntityFactory _entityFactory;

    public Fox(
        IEntityLocator<Animal> entityLocator,
        IEntityLocator<Animal> preyLocator,
        IWorldService worldService,
        ITimeManager timeManager,
        IEntityFactory entityFactory,
        Position position,
        int healthPoints,
        int energy,
        bool isMale)
        : base(
            entityLocator,
            preyLocator,
            worldService,
            timeManager,
            entityFactory,
            position,
            healthPoints,
            energy,
            isMale,
            visionRadius: 0.15,
            contactRadius: 0.01,
            basalMetabolicRate: 1.2)
    {
        _entityFactory = entityFactory;
        MovementSpeed = 2.0;
        Color = Brushes.Orange;
        _territoryCenter = position;

        _environmentPreferences.Clear();
        _environmentPreferences.Add(new EnvironmentPreference(PreferredEnvironment, 1.0, 1.0));
        
        AddBehavior(new HuntingBehavior(worldService, new GroundHuntingStrategy(worldService)));
        // AddBehavior(new TerritorialBehavior(worldService, position));

        try 
        {
            var spritePath = AssetHelper.GetAssetPath("Fox Sprite Sheet.png");
            InitializeSprite(spritePath, 32, 32);

            Console.WriteLine("Initializing Fox animations");

            // Configure animations avec les bonnes lignes
            Sprite?.AddAnimation(AnimationState.Idle, 
                new AnimatedSprite.AnimationConfig(
                    row: 0,
                    startFrame: 0,
                    frameCount: 5,
                    frameDuration: 0.05));

            Sprite?.AddAnimation(AnimationState.IdleAlt, 
                new AnimatedSprite.AnimationConfig(
                    row: 1,
                    startFrame: 0,    
                    frameCount: 14,
                    frameDuration: 0.05));

            Sprite?.AddAnimation(AnimationState.Moving, 
                new AnimatedSprite.AnimationConfig(
                    row: 2,
                    startFrame: 0,    
                    frameCount: 8,
                    frameDuration: 0.05));

            Sprite?.AddAnimation(AnimationState.Catching,
                new AnimatedSprite.AnimationConfig(
                    row: 3,
                    startFrame: 0,
                    frameCount: 11,
                    frameDuration: 0.05));

            Sprite?.AddAnimation(AnimationState.TakingDamage,
                new AnimatedSprite.AnimationConfig(
                    row: 4,
                    startFrame: 0,
                    frameCount: 5,
                    frameDuration: 0.05,
                    loop: false));

            Sprite?.AddAnimation(AnimationState.Sleeping,
                new AnimatedSprite.AnimationConfig(
                    row: 5,
                    startFrame: 0,
                    frameCount: 6,
                    frameDuration: 0.05));
            
            Sprite?.AddAnimation(AnimationState.Dying,
                new AnimatedSprite.AnimationConfig(
                    row: 6,
                    startFrame: 0,
                    frameCount: 7,
                    frameDuration: 0.05,
                    loop: false));

            Console.WriteLine("Fox animations initialized successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load fox sprite: {ex.Message}");
        }

        Console.WriteLine($"Created Fox with color {Color} at {Position.X}, {Position.Y}");
        
    }

    public override Animal CreateOffspring(Position position)
    {
        return _entityFactory.CreateAnimal<Fox>(60, 80, position, RandomHelper.Instance.NextDouble() > 0.5);
    }

    protected override AnimationState DetermineAnimationState()
    {
        if (IsDead) return AnimationState.Dead;
        
        switch (Stats.CurrentBehavior)
        {
            case "Hunt":
                return AnimationState.Catching;
            case "TakeDamage":
                return AnimationState.TakingDamage;
            default:
                double deltaX = Math.Abs(_currentDirectionX);
                double deltaY = Math.Abs(_currentDirectionY);
                bool isMoving = deltaX > 0.01 || deltaY > 0.01;
                return isMoving ? AnimationState.Moving : (RandomHelper.NextDouble() > 0.5 ? AnimationState.Idle : AnimationState.IdleAlt);
        }
    }

    public override void Update()
    {
        base.Update();
        UpdateAnimation(_timeManager.DeltaTime);
    }
}

