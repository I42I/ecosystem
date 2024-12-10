using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Core;

namespace ecosystem.Models.Behaviors.Reproduction;

public interface IReproducible
{
    bool IsMale { get; }
    double ReproductionEnergyCost { get; }
    Animal? FindNearestMate();
    void Reproduce(Animal partner);
    Position Position { get; }
    double ContactRadius { get; }
}
