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

public class Grass : Plant, IStaticSpriteEntity
{
    public ISprite? Sprite { get; private set; }
    public static int DefaultMaxHealth => 50;
    public static int DefaultMaxEnergy => 200;
    public override int MaxHealth => DefaultMaxHealth;
    public override int MaxEnergy => DefaultMaxEnergy;
    protected override double BaseAbsorptionRate => 0.2;
    private readonly IEntityFactory _entityFactory;


    public Grass(
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
            basalMetabolicRate: 0.5,
            environment: EnvironmentType.Ground,
            rootRadius: 0.1,
            seedRadius: 0.2,
            contactRadius: 0.02,
            worldService: worldService,
            timeManager: timeManager)
    {
        _entityFactory = entityFactory;
        Color = Brushes.Green;

        try
        {
            Sprite = new StaticSprite(AssetHelper.GetAssetPath("Fern1_1.png"));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load meat sprite: {ex.Message}");
        }
    }

    public override EnvironmentType PreferredEnvironment => EnvironmentType.Ground;

    protected override bool CanSpreadSeeds()
    {
        return base.CanSpreadSeeds() && 
            Energy >= SimulationConstants.PLANT_MIN_ENERGY_FOR_REPRODUCTION && 
            HealthPoints >= SimulationConstants.PLANT_MIN_HEALTH_FOR_REPRODUCTION;
    }

    protected override Plant CreateOffspring(Position position)
    {
        return _entityFactory.CreatePlant<Grass>(30, 50, position);
    }
}
