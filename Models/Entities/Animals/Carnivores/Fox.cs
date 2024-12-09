using System;
using System.Linq;
using System.Collections.Generic;
using Avalonia.Media;
using ecosystem.Helpers;
using ecosystem.Models.Behaviors;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Entities.Animals.Herbivores;
using ecosystem.Services.World;
using ecosystem.Models.Core;

namespace ecosystem.Models.Entities.Animals.Carnivores;

public class Fox : Carnivore
{
    protected override double BaseAttackPower => 15.0;
    protected override double BaseAttackRange => 1.5;
    protected override double BaseHungerThreshold => 40.0;
    protected override double BaseReproductionThreshold => 60.0;
    public override EnvironmentType PreferredEnvironment => EnvironmentType.Ground;

    public Fox(
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
            visionRadius: 10.0,
            contactRadius: 2.0,
            basalMetabolicRate: 1.2)
    {
        Color = new SolidColorBrush(Colors.Red);
        Console.WriteLine($"Created Fox with color Red at {Position.X}, {Position.Y}");
        
    }

    public override Animal? FindNearestPrey()
    {
        // Logique spécifique pour trouver des proies terrestres
        return null;
    }

    public override void MoveTowardsPrey(Animal prey)
    {
        // Logique de déplacement terrestre
        double distance = GetDistanceTo(prey);
        if (distance > 0)
        {
            double dx = (prey.Position.X - Position.X) / distance;
            double dy = (prey.Position.Y - Position.Y) / distance;
            Move(dx, dy);
        }
    }

    protected override void GiveBirth()
    {
        var (x, y) = RandomHelper.GetRandomPositionInRadius(Position.X, Position.Y, 1.0);
        // Création d'un nouveau renard avec des caractéristiques héritées
    }

    public override void SearchForMate()
    {
        // Find closest fox of opposite gender in vision range
        // If found and in contact range, call Reproduce
        // Implementation example:
        // var mate = FindNearestMate();
        // if (mate != null && IsInContactWith(mate))
        // {
        //     Reproduce(mate);
        // }
    }

    protected override int CalculateAttackDamage()
    {
        // Le dégât de base plus un bonus aléatoire
        return (int)(BaseAttackPower * (0.8 + RandomHelper.NextDouble() * 0.4));
    }

    protected override int CalculateEnergyGain()
    {
        // L'énergie gagnée est proportionnelle à l'attaque
        return (int)(BaseAttackPower * 0.5);
    }

    protected override IEnumerable<Animal> GetPotentialPrey()
    {
        return _entityLocator.FindInRadius(
            _worldService.Entities.OfType<Rabbit>(),
            VisionRadius);
    }
}

