using ecosystem.Models.Core;

namespace ecosystem.Models.Entities.Environment;

public class OrganicWaste : LifeForm
{
    public int EnergyValue { get; }

    public OrganicWaste((double X, double Y) position, int energyValue)
        : base(
            healthPoints: 0, 
            energy: 0, 
            position, 
            basalMetabolicRate: 0,
            environment: EnvironmentType.Ground | EnvironmentType.Water)
    {
        EnergyValue = energyValue;
    }

    public override EnvironmentType PreferredEnvironment => EnvironmentType.Ground | EnvironmentType.Water;

    protected override void UpdateBehavior()
    {
        // Les déchets organiques n'ont pas de comportement actif
    }

    protected override void OnDeath()
    {
        // Les déchets organiques ne meurent pas
    }
}
