using System.Linq;
using ecosystem.Models.Behaviors.Base;
using ecosystem.Models.Entities.Animals;

namespace ecosystem.Models.Behaviors.Movement;

public class GroupMovementBehavior : IBehavior<Animal>
{
    private readonly double _groupRadius;
    private readonly double _separationDistance;

    public GroupMovementBehavior(double groupRadius = 10.0, double separationDistance = 2.0)
    {
        _groupRadius = groupRadius;
        _separationDistance = separationDistance;
    }

    public int Priority => 1;

    public bool CanExecute(Animal animal)
    {
        var nearbyAnimals = animal.GetNearbyEntities(_groupRadius)
            .OfType<Animal>()
            .Where(a => a.GetType() == animal.GetType());
        return nearbyAnimals.Any();
    }

    public void Execute(Animal animal)
    {
        var neighbors = animal.GetNearbyEntities(_groupRadius)
            .OfType<Animal>()
            .Where(a => a.GetType() == animal.GetType());

        var centerX = neighbors.Average(n => n.Position.X);
        var centerY = neighbors.Average(n => n.Position.Y);

        // Move towards center while maintaining separation
        var dx = centerX - animal.Position.X;
        var dy = centerY - animal.Position.Y;

        // Add separation force
        foreach (var neighbor in neighbors)
        {
            var distance = animal.GetDistanceTo(neighbor.Position);
            if (distance < _separationDistance)
            {
                dx -= (neighbor.Position.X - animal.Position.X);
                dy -= (neighbor.Position.Y - animal.Position.Y);
            }
        }

        animal.Move(dx, dy);
    }
}