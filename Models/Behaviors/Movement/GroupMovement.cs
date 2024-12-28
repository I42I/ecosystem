using System;
using System.Linq;
using System.Collections.Generic;
using ecosystem.Models.Behaviors.Base;
using ecosystem.Models.Entities.Animals;
using ecosystem.Services.World;
using ecosystem.Services.Simulation;
using ecosystem.Helpers;

namespace ecosystem.Models.Behaviors.Movement;

public class GroupMovementBehavior : IBehavior<Animal>
{
    private readonly IWorldService _worldService;
    private readonly double _groupRadius;
    private readonly double _separationDistance;
    private double _behaviorAccumulator;
    private const double GROUP_CHECK_INTERVAL = 3.0;
    private const double ISOLATION_THRESHOLD = 0.15;

    public GroupMovementBehavior(IWorldService worldService, double groupRadius = 0.2, double separationDistance = 0.04)
    {
        _worldService = worldService;
        _groupRadius = groupRadius;
        _separationDistance = separationDistance;
        _behaviorAccumulator = 0;
    }

    public string Name => "GroupMovement";
    public int Priority => 1;

    public bool CanExecute(Animal animal)
    {
        _behaviorAccumulator += SimulationConstants.BEHAVIOR_UPDATE_INTERVAL;

        if (_behaviorAccumulator < GROUP_CHECK_INTERVAL)
            return false;

        _behaviorAccumulator = 0;

        var nearbyAnimals = _worldService.GetEntitiesInRange(animal.Position, animal.VisionRadius)
            .OfType<Animal>()
            .Where(a => a.GetType() == animal.GetType())
            .ToList();

        return nearbyAnimals.Any() && IsIsolated(animal, nearbyAnimals);
    }

    private bool IsIsolated(Animal animal, List<Animal> neighbors)
    {
        if (!neighbors.Any()) return true;

        var avgDistance = neighbors
            .Average(n => animal.GetDistanceTo(n.Position));

        return avgDistance > ISOLATION_THRESHOLD;
    }

    public void Execute(Animal animal)
    {
        var neighbors = _worldService.GetEntitiesInRange(animal.Position, animal.VisionRadius)
            .OfType<Animal>()
            .Where(a => a.GetType() == animal.GetType())
            .ToList();

        if (!neighbors.Any())
            return;

        var closestGroup = FindClosestGroup(neighbors, animal);
        if (!closestGroup.Any()) return;

        var centerX = closestGroup.Average(n => n.Position.X);
        var centerY = closestGroup.Average(n => n.Position.Y);

        double dx = centerX - animal.Position.X;
        double dy = centerY - animal.Position.Y;

        var (randX, randY) = RandomHelper.GetRandomDirection();
        dx += randX * 0.2;
        dy += randY * 0.2;

        var length = Math.Sqrt(dx * dx + dy * dy);
        if (length > 0)
        {
            var moveSpeed = animal.MovementSpeed * 0.5;
            animal.MovementSpeed = moveSpeed;
            animal.Move(dx / length, dy / length);
            animal.MovementSpeed = moveSpeed * 2;
        }
    }

        private List<Animal> FindClosestGroup(List<Animal> neighbors, Animal animal)
    {
        var groups = new List<List<Animal>>();
        var remainingAnimals = new List<Animal>(neighbors);

        while (remainingAnimals.Any())
        {
            var currentGroup = new List<Animal>();
            var startAnimal = remainingAnimals[0];
            currentGroup.Add(startAnimal);
            remainingAnimals.RemoveAt(0);

            for (int i = remainingAnimals.Count - 1; i >= 0; i--)
            {
                if (currentGroup.Any(g => g.GetDistanceTo(remainingAnimals[i].Position) < _separationDistance * 3))
                {
                    currentGroup.Add(remainingAnimals[i]);
                    remainingAnimals.RemoveAt(i);
                }
            }

            groups.Add(currentGroup);
        }

        return groups
            .OrderBy(g => g.Average(a => animal.GetDistanceTo(a.Position)))
            .FirstOrDefault() ?? new List<Animal>();
    }
}