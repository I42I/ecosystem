using System.Linq;
using ecosystem.Models.Core;
using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Entities.Animals.Carnivores;
using ecosystem.Models.Behaviors.Base;
using ecosystem.Services.World;


namespace ecosystem.Models.Behaviors.Survival;

public class FleeingBehavior : IBehavior<Animal>
{
    private readonly IWorldService _worldService;

    public FleeingBehavior(IWorldService worldService)
    {
        _worldService = worldService;
    }

    public string Name => "Fleeing";
    public int Priority => 3;

    public bool CanExecute(Animal animal)
    {
        var predators = _worldService.GetEntitiesInRange(animal.Position, animal.VisionRadius)
            .OfType<Carnivore>()
            .Where(p => p != animal);

        return predators.Any();
    }

    public void Execute(Animal animal)
    {
        var predators = _worldService.GetEntitiesInRange(animal.Position, animal.VisionRadius)
            .OfType<Carnivore>()
            .Where(p => p != animal);

        var nearestPredator = predators
            .OrderBy(p => animal.GetDistanceTo(p.Position))
            .FirstOrDefault();

        if (nearestPredator != null)
        {
            double dx = animal.Position.X - nearestPredator.Position.X;
            double dy = animal.Position.Y - nearestPredator.Position.Y;

            var length = System.Math.Sqrt(dx * dx + dy * dy);
            if (length > 0)
            {
                dx /= length;
                dy /= length;
            }

            animal.Move(dx, dy);
        }
    }
}