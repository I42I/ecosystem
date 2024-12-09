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
    }

    public override EnvironmentType PreferredEnvironment => EnvironmentType.Ground;

    protected override bool CanSpreadSeeds()
    {
        return Energy > 50 && HealthPoints > 50;
    }

    protected override Plant CreateOffspring(Position position)
    {
        var offspringPosition = new Position(position.X, position.Y);
        return new Grass(
            healthPoints: HealthPoints / 2,
            energy: Energy / 2,
            position: offspringPosition
        );
    }

    protected override void OnDeath()
    {
        // Transform into organic waste when dies
        // This should be handled by the WorldService
    }
}
