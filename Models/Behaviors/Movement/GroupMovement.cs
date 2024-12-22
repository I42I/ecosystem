using System.Linq;
using ecosystem.Models.Behaviors.Base;
using ecosystem.Models.Entities.Animals;
using ecosystem.Services.World;

namespace ecosystem.Models.Behaviors.Movement;

public class GroupMovementBehavior : IBehavior<Animal>
{
    private readonly IWorldService _worldService;
    private readonly double _groupRadius;
    private readonly double _separationDistance;

    public GroupMovementBehavior(IWorldService worldService, double groupRadius = 0.2, double separationDistance = 0.04)
    {
        _worldService = worldService;
        _groupRadius = groupRadius;
        _separationDistance = separationDistance;
    }

    public string Name => "GroupMovement";
    public int Priority => 1;

    public bool CanExecute(Animal animal)
    {
        var nearbyAnimals = _worldService.GetEntitiesInRange(animal.Position, _groupRadius)
            .OfType<Animal>()
            .Where(a => a.GetType() == animal.GetType());

        return nearbyAnimals.Any();
    }

    public void Execute(Animal animal)
    {
        var neighbors = _worldService.GetEntitiesInRange(animal.Position, _groupRadius)
            .OfType<Animal>()
            .Where(a => a.GetType() == animal.GetType())
            .ToList();

        if (!neighbors.Any())
            return;

        var centerX = neighbors.Average(n => n.Position.X);
        var centerY = neighbors.Average(n => n.Position.Y);

        double dx = centerX - animal.Position.X;
        double dy = centerY - animal.Position.Y;

        foreach (var neighbor in neighbors)
        {
            var distance = animal.GetDistanceTo(neighbor.Position);
            if (distance < _separationDistance)
            {
                dx -= (neighbor.Position.X - animal.Position.X);
                dy -= (neighbor.Position.Y - animal.Position.Y);
            }
        }

        var length = System.Math.Sqrt(dx * dx + dy * dy);
        if (length > 0)
        {
            dx /= length;
            dy /= length;
        }

        animal.Move(dx, dy);
    }
}