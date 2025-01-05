using System;
using Avalonia.Media;
using ecosystem.Helpers;
using ecosystem.Models.Behaviors.Reproduction;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Behaviors.Hunt;
using ecosystem.Services.World;
using ecosystem.Models.Core;
using ecosystem.Services.Simulation;
using ecosystem.Services.Factory;
using ecosystem.Models.Animation;

namespace ecosystem.Models.Entities.Animals.Carnivores;

public class Shark : Carnivore, IReproductionConstants
{
    public static int DefaultMaxHealth => 150;
    public static int DefaultMaxEnergy => 200;
    public override int MaxHealth => DefaultMaxHealth;
    public override int MaxEnergy => DefaultMaxEnergy;
    public override double BaseAttackPower => 25.0;
    protected override double BaseAttackRange => 0.015;
    protected override double BaseBiteCooldownDuration => 0.05;
    public override double BaseHungerThreshold => 60.0;
    protected override double BaseReproductionThreshold => 70.0;
    protected override double BaseReproductionEnergyCost => 40.0;
    protected override double SpeciesEnergyCostModifier => 1.2;
    public static EnvironmentType DefaultEnvironment => EnvironmentType.Water;
    public override EnvironmentType PreferredEnvironment => DefaultEnvironment;

    public double GestationPeriod => 0.4;
    public double MaleReproductionCooldown => 15.0;
    public double FemaleReproductionCooldown => 20.0;

    private readonly IEntityFactory _entityFactory;

    public Shark(
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
            contactRadius: 0.012,
            basalMetabolicRate: 1.5)
    {
        _entityFactory = entityFactory;
        MovementSpeed = 1.8;
        Color = Brushes.DarkGray;

        _environmentPreferences.Clear();
        _environmentPreferences.Add(new EnvironmentPreference(
            PreferredEnvironment,
            movementMod: 1.0,
            energyLossMod: 1.0
        ));
        _environmentPreferences.Add(new EnvironmentPreference(
            EnvironmentType.Ground,
            movementMod: 0.0,
            energyLossMod: 10.0
        ));

        AddBehavior(new HuntingBehavior(worldService, new WaterHuntingStrategy(worldService)));

        try 
        {
            var spritePath = AssetHelper.GetAssetPath("Shark.png");
            InitializeSprite(spritePath, 32, 32);

            Console.WriteLine("Initializing Shark animations");

            Sprite?.AddAnimation(AnimationState.Idle, 
                new AnimatedSprite.AnimationConfig(
                    row: 0,
                    startFrame: 0,
                    frameCount: 8,
                    frameDuration: 0.03
                ));

            Sprite?.AddAnimation(AnimationState.Moving,
                new AnimatedSprite.AnimationConfig(
                    row: 0,
                    startFrame: 0,
                    frameCount: 8,
                    frameDuration: 0.03
                ));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load shark sprite: {ex.Message}");
        }
        
        Console.WriteLine($"Created Shark with color {Color} at {Position.X}, {Position.Y}");
    }

    public override Animal CreateOffspring(Position position)
    {
        return _entityFactory.CreateAnimal<Shark>(60, 80, position, RandomHelper.Instance.NextDouble() > 0.5);
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
