using System;
using System.Collections.Generic;
using ecosystem.Models;
using ecosystem.Models.Behaviors;
using ecosystem.Helpers;
using System.Linq;
using ecosystem.Models.Entities.Plants;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Core;
using ecosystem.Services.World;

namespace ecosystem.Models.Entities.Animals.Herbivores;

public abstract class Herbivore : Animal
{
    protected Herbivore(
        IEntityLocator<Animal> entityLocator,
        IWorldService worldService,
        Position position,
        int healthPoints,
        int energy,
        bool isMale,
        double visionRadius,
        double contactRadius,
        double basalMetabolicRate)
        : base(entityLocator, worldService, position, healthPoints, energy, isMale,
               visionRadius, contactRadius, basalMetabolicRate, EnvironmentType.Ground)
    {
    }

    protected override void SearchForFood()
    {
        var nearestPlant = FindNearestPlant();
        if (nearestPlant != null && IsInContactWith(nearestPlant))
        {
            Eat(nearestPlant);
        }
        else if (nearestPlant != null)
        {
            MoveTowardsFood(nearestPlant);
        }
    }

    protected virtual Plant? FindNearestPlant()
    {
        return null;
    }

    protected virtual void MoveTowardsFood(Plant plant)
    {
        var dx = plant.Position.X - Position.X;
        var dy = plant.Position.Y - Position.Y;
        var distance = Math.Sqrt(dx * dx + dy * dy);

        if (distance > 0)
        {
            Move(dx / distance, dy / distance);
        }
    }

    protected bool IsInContactWith(Plant plant)
    {
        var dx = plant.Position.X - Position.X;
        var dy = plant.Position.Y - Position.Y;
        var distance = Math.Sqrt(dx * dx + dy * dy);
        return distance <= ContactRadius;
    }

    protected virtual void Eat(Plant plant)
    {
        Energy += 10;
    }

    protected override int CalculateMovementEnergyCost(double deltaX, double deltaY)
    {
        double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        return (int)(distance * GetEnvironmentMovementModifier());
    }
}