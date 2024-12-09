using System;
using Avalonia.Media;
using System.Linq;
using ecosystem.Models.Entities.Environment;
using ecosystem.Helpers;
using ecosystem.Models.Behaviors;
using ecosystem.Models.Entities.Plants;
using ecosystem.Services.World;
using ecosystem.Models.Core;
using ecosystem.Models.Entities.Animals.Carnivores;

namespace ecosystem.Models.Entities.Animals.Herbivores;

public class Rabbit : Herbivore
{
    private readonly IEntityLocator<Plant> _plantLocator;
    protected override double BaseHungerThreshold => 55.0;
    protected override double BaseReproductionThreshold => 70.0;
    protected override double BaseReproductionEnergyCost => 20.0;
    public override EnvironmentType PreferredEnvironment => EnvironmentType.Ground;

    public Rabbit(
        IEntityLocator<Animal> entityLocator,
        IEntityLocator<Plant> plantLocator,
        IWorldService worldService,
        int healthPoints,
        int energy,
        Position position,
        bool isMale)
        : base(
            entityLocator,
            worldService,
            position,
            healthPoints,
            energy,
            isMale,
            visionRadius: 8.0,
            contactRadius: 1.0,
            basalMetabolicRate: 0.8)
    {
        MovementSpeed = 1.5;
        _plantLocator = plantLocator;
        HungerThreshold = BaseHungerThreshold;
        ReproductionEnergyThreshold = BaseReproductionThreshold;
        ReproductionEnergyCost = BaseReproductionEnergyCost;
        Color = new SolidColorBrush(Colors.Brown);
        Console.WriteLine($"Created Rabbit with color {Color} at {Position.X}, {Position.Y}");
    }

    protected override Plant? FindNearestPlant()
    {
        return _plantLocator.FindNearest(
            _worldService.Entities.OfType<Plant>(),
            VisionRadius);
    }

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
            _entityLocator,
            _plantLocator, 
            _worldService,
            healthPoints: 50,
            energy: 50,
            position: new Position(x, y),
            isMale: RandomHelper.NextDouble() > 0.5
        );
    }
}
