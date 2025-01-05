using System;
using Avalonia.Media;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Core;
using ecosystem.Services.World;
using ecosystem.Services.Simulation;
using ecosystem.Services.Factory;

namespace ecosystem.Models.Entities.Plants;

public class Algae : Plant
{
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
            contactRadius: 0.02,
            worldService: worldService,
            timeManager: timeManager)
    {
        _entityFactory = entityFactory;
        Color = Brushes.LightSeaGreen;
    }

    protected override bool CanSpreadSeeds()
    {
        return Energy > 30 && HealthPoints > 40;
    }

    protected override Plant CreateOffspring(Position position)
    {
        return _entityFactory.CreatePlant<Algae>(30, 50, position);
    }
}
