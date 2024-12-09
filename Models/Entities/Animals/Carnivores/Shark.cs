using Avalonia.Media;
using ecosystem.Models.Entities.Environment;
using ecosystem.Helpers;
using System.Collections.Generic;
using System.Linq;
using ecosystem.Models.Behaviors;
using ecosystem.Models.Core;
using ecosystem.Services.World;

namespace ecosystem.Models.Entities.Animals.Carnivores;

public class Shark : Carnivore
{
    protected override double BaseAttackPower => 25.0;
    protected override double BaseAttackRange => 2.0;
    protected override double BaseHungerThreshold => 50.0;
    protected override double BaseReproductionThreshold => 70.0;
    protected override double BaseReproductionEnergyCost => 40.0;
    public override EnvironmentType PreferredEnvironment => EnvironmentType.Water;

    public Shark(
        IEntityLocator<Animal> entityLocator,
        IEntityLocator<Animal> preyLocator,
        IWorldService worldService,
        int healthPoints,
        int energy,
        Position position,
        bool isMale)
        : base(
            entityLocator,
            preyLocator,
            worldService,
            position,
            healthPoints,
            energy,
            isMale,
            visionRadius: 15.0,
            contactRadius: 3.0,
            basalMetabolicRate: 1.5)
    {
        Color = new SolidColorBrush(Colors.Gray);
    }

    public override double GetEnvironmentMovementModifier()
    {
        return Environment.HasFlag(PreferredEnvironment) ? 0.8 : 2.0;
    }

    public override Animal? FindNearestPrey()
    {
        return null;
    }

    public override void MoveTowardsPrey(Animal prey)
    {
        double distance = GetDistanceTo(prey);
        if (distance > 0)
        {
            double dx = (prey.Position.X - Position.X) / distance;
            double dy = (prey.Position.Y - Position.Y) / distance;
            Move(dx * 1.5, dy * 1.5);
        }
    }

    protected override int CalculateAttackDamage()
    {
        return Environment.HasFlag(PreferredEnvironment) 
            ? (int)(BaseAttackPower * 1.5)
            : (int)(BaseAttackPower * 0.5);
    }

    protected override int CalculateEnergyGain()
    {
        return (int)(BaseAttackPower * 0.8);
    }

    public override void SearchForMate()
    {
        // Find closest shark of opposite gender in vision range
        // If found and in contact range, call Reproduce
        // Implementation example:
        // var mate = FindNearestMate();
        // if (mate != null && IsInContactWith(mate))
        // {
        //     Reproduce(mate);
        // }
    }

    protected override void GiveBirth()
    {
        var (x, y) = RandomHelper.GetRandomPositionInRadius(Position.X, Position.Y, 2.0);
        // Create new shark with inherited characteristics
        // Implementation example:
        // var baby = new Shark(
        //     healthPoints: HealthPoints / 2,
        //     energy: Energy / 2,
        //     position: (x, y),
        //     isMale: RandomHelper.NextDouble() > 0.5
        // );
        // World.Instance.AddAnimal(baby);
    }

    protected override IEnumerable<Animal> GetPotentialPrey()
    {
        // Dans le cas du requin, les proies potentielles sont les animaux aquatiques
        // Cette implémentation devrait utiliser le WorldService ou EntityLocator
        // pour trouver les proies dans l'eau
        return Enumerable.Empty<Animal>();
    }

    // ...implementation specific to sharks...
}
