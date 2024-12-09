using System;
using ecosystem.Models.Core;

namespace ecosystem.Models.Entities.Environment;

public class OrganicWaste : LifeForm
{
    private int _energyValue;
    public int EnergyValue 
    { 
        get => _energyValue;
        set => _energyValue = Math.Max(0, value);
    }

    public OrganicWaste(Position position, int energyValue)
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
