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
using ecosystem.Models.Entities.Animals.Carnivores;
using ecosystem.Models.Behaviors.Survival;

namespace ecosystem.Models.Entities.Animals.Herbivores;

public abstract class Herbivore : Animal, IFleeingEntity
{
    protected readonly IEntityLocator<Plant> _plantLocator;
    protected abstract double BaseHungerThreshold { get; }
    protected abstract double BaseReproductionThreshold { get; }
    protected abstract double BaseReproductionEnergyCost { get; }
    protected abstract int BaseBiteSize { get; }

    protected Herbivore(
        IEntityLocator<Animal> entityLocator,
        IEntityLocator<Plant> plantLocator,
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
        _plantLocator = plantLocator;
    }

    protected Plant? FindNearestPlant()
    {
        return _plantLocator.FindNearest(
            _worldService.Entities.OfType<Plant>(),
            VisionRadius
        );
    }

    public override void SearchForFood()
    {
        var nearestPlant = FindNearestPlant();
        if (nearestPlant != null)
        {
            double dx = nearestPlant.Position.X - Position.X;
            double dy = nearestPlant.Position.Y - Position.Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);

            if (distance > 0)
            {
                Move(dx/distance, dy/distance);
                
                if (IsInContactWith(nearestPlant))
                {
                    Eat(nearestPlant);
                }
            }
        }
        else
        {
            Rest();
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
        if (!plant.IsDead)
        {
            int energyGained = BaseBiteSize / 2;
            plant.TakeDamage(BaseBiteSize);
            Energy += energyGained;
        }
    }

    protected override int CalculateMovementEnergyCost(double deltaX, double deltaY)
    {
        double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        return (int)(distance * GetEnvironmentMovementModifier());
    }

    protected override void UpdateBehavior()
    {
        var nearbyPredators = _worldService.Entities
            .OfType<Carnivore>()
            .Where(c => GetDistanceTo(c.Position) <= VisionRadius * 1.5)
            .ToList();

        if (nearbyPredators.Any())
        {
            FleeFromPredator(nearbyPredators.OrderBy(p => GetDistanceTo(p.Position)).First());
            return;
        }

        base.UpdateBehavior();
    }

    public IEnumerable<Entity> GetNearbyEntities(double radius)
    {
        return WorldService.GetEntitiesInRange(Position, radius);
    }

    public void FleeFromPredator(Animal predator)
    {
        _directionChangeTicks = 0;

        double dx = Position.X - predator.Position.X;
        double dy = Position.Y - predator.Position.Y;
        double distance = Math.Sqrt(dx * dx + dy * dy);

        if (distance > 0)
        {
            _currentDirectionX = dx / distance;
            _currentDirectionY = dy / distance;
            Move(_currentDirectionX * MovementSpeed, _currentDirectionY * MovementSpeed);
        }
    }
}