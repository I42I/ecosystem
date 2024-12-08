using ecosystem.Models.Entities.Environment;

namespace ecosystem.Models.Behaviors;

public interface IEnvironmentSensitive
{
    EnvironmentType Environment { get; }
    EnvironmentType PreferredEnvironment { get; }
    double GetEnvironmentMovementModifier();
}
