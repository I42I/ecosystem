using System;
using Avalonia.Media;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Core;
using ecosystem.Services.World;
using ecosystem.Services.Simulation;

namespace ecosystem.Models.Entities.Plants;

public class Algae : Plant
{
    protected override double BaseAbsorptionRate => 0.3;

    public Algae(
        IWorldService worldService, 
        ITimeManager timeManager,
        int healthPoints, 
        int energy, 
        Position position)
        : base(
            healthPoints,
            energy,
            position,
            basalMetabolicRate: 0.4,
            environment: EnvironmentType.Water,
            worldService: worldService,
            timeManager: timeManager)
    {
        RootRadius = 3.0;
        SeedRadius = 8.0;
        Color = Brushes.LightSeaGreen;
    }

    public override EnvironmentType PreferredEnvironment => EnvironmentType.Water;

    protected override bool CanSpreadSeeds()
    {
        return Energy > 30 && HealthPoints > 40;
    }

    protected override Plant CreateOffspring(Position position)
    {
        return new Algae(
            worldService: _worldService,
            timeManager: _timeManager,
            healthPoints: HealthPoints / 2,
            energy: Energy / 2,
            position: new Position(position.X, position.Y)
        );
    }
}
