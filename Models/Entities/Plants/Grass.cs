using System;
using Avalonia.Media;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Core;

namespace ecosystem.Models.Entities.Plants;

public class Grass : Plant
{
    public Grass(int healthPoints, int energy, Position position)
        : base(
            healthPoints,
            energy,
            position,
            basalMetabolicRate: 0.5,
            environment: EnvironmentType.Ground)
    {
        RootRadius = 5.0;
        SeedRadius = 10.0;
        Color = new SolidColorBrush(Colors.Green);
        Console.WriteLine($"Created Grass with color {Color} at {Position.X}, {Position.Y}");
    }

    public override EnvironmentType PreferredEnvironment => EnvironmentType.Ground;

    protected override bool CanSpreadSeeds()
    {
        return Energy > 50 && HealthPoints > 50;
    }

    protected override Plant CreateOffspring(Position position)
    {
        return new Grass(
            healthPoints: HealthPoints / 2,
            energy: Energy / 2,
            position: new Position(position.X, position.Y)
        );
    }

    protected override void OnDeath()
    {
        // Transform into organic waste when dies
        // This should be handled by the WorldService
    }
}
