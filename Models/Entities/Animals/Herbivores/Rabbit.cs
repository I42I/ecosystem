using Avalonia.Media;
using ecosystem.Models.Entities.Environment;
using ecosystem.Helpers;
using ecosystem.Models.Behaviors;

namespace ecosystem.Models.Entities.Animals.Herbivores;

public class Rabbit : Herbivore
{
    private readonly IEntityLocator<Animal> _entityLocator;

    public Rabbit(
        IEntityLocator<Animal> entityLocator,
        int healthPoints,
        int energy,
        (double X, double Y) position,
        bool isMale)
        : base(
            entityLocator,
            healthPoints,
            energy,
            position,
            isMale,
            visionRadius: 8.0,
            contactRadius: 1.0,
            basalMetabolicRate: 0.8,
            environment: EnvironmentType.Ground)
    {
        _entityLocator = entityLocator;
        Color = new SolidColorBrush(Colors.Brown);
        HungerThreshold = 30;
        ReproductionEnergyThreshold = 70;
    }

    public override EnvironmentType PreferredEnvironment => EnvironmentType.Ground;

    public override void SearchForMate()
    {
        // Implémenter la recherche de partenaire avec WorldService
    }

    protected override void SearchForFood()
    {
        // Recherche de nourriture pour les herbivores
    }

    protected override void GiveBirth()
    {
        var (x, y) = RandomHelper.GetRandomPositionInRadius(Position.X, Position.Y, 2.0);
        var baby = new Rabbit(
            entityLocator: _entityLocator,
            healthPoints: 50,
            energy: 50,
            position: (x, y),
            isMale: RandomHelper.NextDouble() > 0.5
        );
    }
}
