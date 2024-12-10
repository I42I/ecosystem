using ecosystem.Models.Core;
using ecosystem.Models.Entities.Animals;

namespace ecosystem.Models.Behaviors.Reproduction;

public interface IMating
{
    bool IsMale { get; }
    double ReproductionEnergyThreshold { get; }
    Animal CreateOffspring(Position position);
}