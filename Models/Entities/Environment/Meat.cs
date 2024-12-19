using Avalonia.Media;
using ecosystem.Models.Core;
using ecosystem.Services.Simulation;

namespace ecosystem.Models.Entities.Environment;

public class Meat : LifeForm
{

    public Meat(Position position, int energyValue, ITimeManager timeManager)
        : base(position, energyValue, energyValue, EnvironmentType.Ground, timeManager)
    {
        Color = Brushes.Red;
    }

    protected override void UpdateBehavior()
    {
        TakeDamage(1);
    }

    protected override void Die()
    {
        var waste = new OrganicWaste(Position, Energy);
    }
}
