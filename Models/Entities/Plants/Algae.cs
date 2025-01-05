using System;
using Avalonia.Media;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Core;
using ecosystem.Services.World;
using ecosystem.Services.Simulation;
using ecosystem.Services.Factory;
using ecosystem.Models.Animation;
using ecosystem.Helpers;

namespace ecosystem.Models.Entities.Plants;

public class Algae : Plant, IAnimatable
{
    public AnimatedSprite? Sprite { get; protected set; }
    public AnimationState CurrentState { get; protected set; }
    protected IAnimationManager? _animationManager;

    public static int DefaultMaxHealth => 70;
    public static int DefaultMaxEnergy => 200;
    public override int MaxHealth => DefaultMaxHealth;
    public override int MaxEnergy => DefaultMaxEnergy;
    protected override double BaseAbsorptionRate => 0.3;
    public static EnvironmentType DefaultEnvironment => EnvironmentType.Water;
    public override EnvironmentType PreferredEnvironment => DefaultEnvironment;
    private readonly IEntityFactory _entityFactory;

    public Algae(
        IWorldService worldService, 
        ITimeManager timeManager,
        IEntityFactory entityFactory,
        int healthPoints, 
        int energy, 
        Position position)
        : base(
            healthPoints,
            energy,
            position,
            basalMetabolicRate: 0.4,
            environment: EnvironmentType.Water,
            rootRadius: 0.1,
            seedRadius: 0.2,
            contactRadius: 0.01,
            worldService: worldService,
            timeManager: timeManager)
    {
        _entityFactory = entityFactory;
        Color = Brushes.LightSeaGreen;

        try 
        {
            var spritePath = AssetHelper.GetAssetPath("Algae.png");
            InitializeSprite(spritePath, 300, 300);
            Console.WriteLine("Initializing Algae animations");

            Sprite?.AddAnimation(AnimationState.Idle, 
                new AnimatedSprite.AnimationConfig(
                    row: 0,
                    startFrame: 0,
                    frameCount: 19,
                    frameDuration: 0.01
                ));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load algae sprite: {ex.Message}");
        }
    }

    protected override bool CanSpreadSeeds()
    {
        return base.CanSpreadSeeds() && 
            Energy >= SimulationConstants.PLANT_MIN_ENERGY_FOR_REPRODUCTION && 
            HealthPoints >= SimulationConstants.PLANT_MIN_HEALTH_FOR_REPRODUCTION;
    }

    protected override Plant CreateOffspring(Position position)
    {
        return _entityFactory.CreatePlant<Algae>(30, 50, position);
    }

    protected void InitializeSprite(string path, int frameWidth, int frameHeight)
    {
        Sprite = new AnimatedSprite(path, frameWidth, frameHeight);
        _animationManager = new AnimationManager(Sprite);
    }

    public void UpdateAnimation(double deltaTime)
    {
        if (_animationManager == null) return;

        if (!_animationManager.HasQueuedAnimations)
        {
            _animationManager.PlayAnimation(new AnimationEvent(AnimationState.Idle));
        }
        
        _animationManager.Update(deltaTime);
    }
}