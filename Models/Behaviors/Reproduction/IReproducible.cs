namespace ecosystem.Models.Behaviors;

public interface IReproducible
{
    bool CanReproduce();
    void Reproduce(IReproducible partner);
    bool IsMale { get; }
    double ReproductionEnergyCost { get; }
}
