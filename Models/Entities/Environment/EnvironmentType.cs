using System;

namespace ecosystem.Models.Entities.Environment;

[Flags]
public enum EnvironmentType
{
    None = 0,
    Ground = 1 << 0,
    Water = 1 << 1
}
