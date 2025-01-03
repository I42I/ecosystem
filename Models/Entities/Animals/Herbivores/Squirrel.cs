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

public class Squirrel : Herbivore
{
    public static int DefaultMaxHealth => 60;
    public static int DefaultMaxEnergy => 80;
    public override int MaxHealth => DefaultMaxHealth;
    public override int MaxEnergy => DefaultMaxEnergy;
    protected override int BaseBiteSize => 4;
    protected override double BaseBiteCooldownDuration => 0.02;
    public override double BaseHungerThreshold => 55.0;
    protected override double BaseReproductionThreshold => 70.0;
    protected override double BaseReproductionEnergyCost => 20.0;
    protected override double SpeciesEnergyCostModifier => 0.8;
    public override EnvironmentType PreferredEnvironment => EnvironmentType.Ground;
    private readonly IEntityFactory _entityFactory;


    public Squirrel(
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
            visionRadius: 0.1,
            contactRadius: 0.01,
            basalMetabolicRate: 0.8)
    {
        _entityFactory = entityFactory;
        MovementSpeed = 1.5;
        Color = Brushes.Brown;

        _environmentPreferences.Clear();
        _environmentPreferences.Add(new EnvironmentPreference(PreferredEnvironment, 1.0, 1.0));

        AddBehavior(new FleeingBehavior(_worldService));        // Priority 3
        AddBehavior(new HungerBehavior());                      // Priority 2
        AddBehavior(new GroupMovementBehavior(_worldService));  // Priority 1
        AddBehavior(new RestBehavior());                        // Priority 0

        try 
        {
            var spritePath = AssetHelper.GetAssetPath("Squirrel Sprite Sheet.png");
            InitializeSprite(spritePath, 32, 32);

            Console.WriteLine("Initializing Fox animations");

            Sprite?.AddAnimation(AnimationState.Idle, 
                new AnimatedSprite.AnimationConfig(
                    row: 0,
                    startFrame: 0,
                    frameCount: 6,
                    frameDuration: 0.02));

            Sprite?.AddAnimation(AnimationState.IdleAlt, 
                new AnimatedSprite.AnimationConfig(
                    row: 1,
                    startFrame: 0,    
                    frameCount: 6,
                    frameDuration: 0.02));

            Sprite?.AddAnimation(AnimationState.Moving, 
                new AnimatedSprite.AnimationConfig(
                    row: 2,
                    startFrame: 0,    
                    frameCount: 8,
                    frameDuration: 0.02));

            Sprite?.AddAnimation(AnimationState.Dig,
                new AnimatedSprite.AnimationConfig(
                    row: 3,
                    startFrame: 0,
                    frameCount: 4,
                    frameDuration: 0.02));

            Sprite?.AddAnimation(AnimationState.Eat,
                new AnimatedSprite.AnimationConfig(
                    row: 4,
                    startFrame: 0,
                    frameCount: 2,
                    frameDuration: 0.02,
                    loop: false));

            Sprite?.AddAnimation(AnimationState.TakingDamage,
                new AnimatedSprite.AnimationConfig(
                    row: 5,
                    startFrame: 0,
                    frameCount: 4,
                    frameDuration: 0.02));
            
            Sprite?.AddAnimation(AnimationState.Dying,
                new AnimatedSprite.AnimationConfig(
                    row: 6,
                    startFrame: 0,
                    frameCount: 4,
                    frameDuration: 0.02,
                    loop: false));

            Console.WriteLine("Squirrel animations initialized successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load squirrel sprite: {ex.Message}");
        }

        Console.WriteLine($"Created Squirrel with color {Color} at {Position.X}, {Position.Y}");
    }

    public override Animal CreateOffspring(Position position)
    {
        return _entityFactory.CreateAnimal<Squirrel>(60, 80, position, RandomHelper.Instance.NextDouble() > 0.5);
    }

    protected override AnimationState DetermineAnimationState()
    {
        if (IsDead) return AnimationState.Dead;
        
        switch (Stats.CurrentBehavior)
        {
            case "Eat":
                return AnimationState.Eat;
            case "TakeDamage":
                return AnimationState.TakingDamage;
            default:
                double deltaX = Math.Abs(_currentDirectionX);
                double deltaY = Math.Abs(_currentDirectionY);
                bool isMoving = deltaX > 0.001 || deltaY > 0.001;
                return isMoving ? AnimationState.Moving : (RandomHelper.NextDouble() > 0.5 ? AnimationState.Idle : AnimationState.IdleAlt);
        }
    }

    public override void Update()
    {
        base.Update();
        UpdateAnimation(_timeManager.DeltaTime);
    }
}
