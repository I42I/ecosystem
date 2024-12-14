using System.Collections.Generic;

namespace ecosystem.Models.Entities.Environment;

public class EnvironmentPreference
{
    public EnvironmentType Type { get; }
    public double MovementModifier { get; }
    public double EnergyLossModifier { get; }
    
    public EnvironmentPreference(EnvironmentType type, double movementMod, double energyLossMod)
    {
        Type = type;
        MovementModifier = movementMod;
        EnergyLossModifier = energyLossMod;
    }
}

public interface IEnvironmentSensitive
{
    IReadOnlyList<EnvironmentPreference> PreferredEnvironments { get; }
    EnvironmentType PreferredEnvironment { get; }
    EnvironmentPreference GetBestEnvironmentPreference(EnvironmentType currentEnv);
}
