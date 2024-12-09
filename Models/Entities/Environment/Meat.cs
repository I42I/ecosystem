using Avalonia.Media;
using ecosystem.Models.Core;

namespace ecosystem.Models.Entities.Environment;

public class Meat : LifeForm
{
    public Meat(Position position, int energyValue)
        : base(
            healthPoints: energyValue,
            energy: energyValue,
            position,
            basalMetabolicRate: 0.1,
            environment: EnvironmentType.Ground | EnvironmentType.Water)
    {
        Color = Brushes.Red;
    }

    public override EnvironmentType PreferredEnvironment => EnvironmentType.Ground | EnvironmentType.Water;

    protected override void UpdateBehavior()
    {
        TakeDamage(1);
    }

    protected override void OnDeath()
    {
        var waste = new OrganicWaste(Position, Energy);
    }
}
