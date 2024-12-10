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
    protected override double BaseReproductionEnergyCost => 30.0;
    public override EnvironmentType PreferredEnvironment => EnvironmentType.Ground;
    private double _territoryRadius = 100.0;
    private Position _territoryCenter;

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
        MovementSpeed = 2.0;
        _territoryCenter = position;
        AttackPower = BaseAttackPower;
        AttackRange = BaseAttackRange;
        HungerThreshold = BaseHungerThreshold;
        ReproductionEnergyThreshold = BaseReproductionThreshold;
        ReproductionEnergyCost = BaseReproductionEnergyCost;
        Color = new SolidColorBrush(Colors.Red);
        Console.WriteLine($"Created Fox with color {Color} at {Position.X}, {Position.Y}");
        
    }

    public override Animal? FindNearestPrey()
    {
        // Logique spécifique pour trouver des proies terrestres
        return null;
    }

    public override void MoveTowardsPrey(Animal prey)
    {
        double dx = prey.Position.X - Position.X;
        double dy = prey.Position.Y - Position.Y;
        double distance = Math.Sqrt(dx * dx + dy * dy);

        if (distance > 0)
        {
            Move(dx / distance * 1.5, dy / distance * 1.5);
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
        return _worldService.Entities
            .OfType<Rabbit>()
            .Where(r => !r.IsDead);
    }

    public override void Rest()
    {
        PatrolTerritory();
    }

    protected virtual void PatrolTerritory()
    {
        Console.WriteLine("Patrolling territory");
        if (_directionChangeTicks <= 0)
        {
            double distanceFromCenter = Math.Sqrt(
                Math.Pow(Position.X - _territoryCenter.X, 2) + 
                Math.Pow(Position.Y - _territoryCenter.Y, 2));

            if (distanceFromCenter > _territoryRadius)
            {
                double dx = _territoryCenter.X - Position.X;
                double dy = _territoryCenter.Y - Position.Y;
                double distance = Math.Sqrt(dx * dx + dy * dy);
                
                _currentDirectionX = dx / distance;
                _currentDirectionY = dy / distance;
            }
            else
            {
                double angle = RandomHelper.Instance.NextDouble() * 2 * Math.PI;
                _currentDirectionX = Math.Cos(angle);
                _currentDirectionY = Math.Sin(angle);
            }
            
            _directionChangeTicks = RandomHelper.Instance.Next(120, 300);
        }
        
        _directionChangeTicks--;
        Move(_currentDirectionX, _currentDirectionY);
    }
}

