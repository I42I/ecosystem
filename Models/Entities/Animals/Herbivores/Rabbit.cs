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
    protected override int BaseBiteSize => 8;
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
            plantLocator,
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
        HungerThreshold = BaseHungerThreshold;
        ReproductionEnergyThreshold = BaseReproductionThreshold;
        ReproductionEnergyCost = BaseReproductionEnergyCost;
        Color = new SolidColorBrush(Colors.Brown);
        Console.WriteLine($"Created Rabbit with color {Color} at {Position.X}, {Position.Y}");
    }

    public override void SearchForMate()
    {
        // Implémenter la recherche de partenaire avec WorldService
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

    public override void Rest()
    {
        StayWithGroup();
    }

    private void StayWithGroup()
    {
        var nearbyRabbits = _worldService.Entities
            .OfType<Rabbit>()
            .Where(r => r != this && GetDistanceTo(r.Position) <= VisionRadius)
            .ToList();

        if (nearbyRabbits.Any())
        {
            var centerX = nearbyRabbits.Average(r => r.Position.X);
            var centerY = nearbyRabbits.Average(r => r.Position.Y);
            
            var (rx, ry) = RandomHelper.GetRandomDirection();
            double dx = (centerX - Position.X) * 0.5 + rx * 0.5;
            double dy = (centerY - Position.Y) * 0.5 + ry * 0.5;
            
            double length = Math.Sqrt(dx * dx + dy * dy);
            if (length > 0)
            {
                Move(dx/length * 0.5, dy/length * 0.5);
            }
        }
        else
        {
            base.Rest();
        }
    }
}
