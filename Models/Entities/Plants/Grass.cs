using System;
using Avalonia.Media;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Core;
using ecosystem.Services.World;
using ecosystem.Services.Simulation;

namespace ecosystem.Models.Entities.Plants;

public class Grass : Plant
{
    protected override double BaseAbsorptionRate => 0.2;

    public Grass(
        IWorldService worldService, 
        ITimeManager timeManager,
        int healthPoints, 
        int energy, 
        Position position)
        : base(
            healthPoints,
            energy,
            position,
            basalMetabolicRate: 0.5,
            environment: EnvironmentType.Ground,
            worldService: worldService,
            timeManager: timeManager)
    {
        RootRadius = 5.0;
        SeedRadius = 10.0;
        Color = Brushes.Green;
    }

    public override EnvironmentType PreferredEnvironment => EnvironmentType.Ground;

    protected override bool CanSpreadSeeds()
    {
        return Energy > 50 && HealthPoints > 50;
    }

    protected override Plant CreateOffspring(Position position)
    {
        return new Grass(
            worldService: _worldService,
            timeManager: _timeManager,
            healthPoints: HealthPoints / 2,
            energy: Energy / 2,
            position: new Position(position.X, position.Y)
        );
    }
}
