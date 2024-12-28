using System.Collections.Generic;
using System.Linq;
using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Entities.Animals.Herbivores;
using ecosystem.Services.World;
using ecosystem.Helpers;
using ecosystem.Models.Core;

namespace ecosystem.Models.Behaviors.Hunt;

public class GroundHuntingStrategy : IHuntingStrategy
{
    private readonly IWorldService _worldService;

    public GroundHuntingStrategy(IWorldService worldService)
    {
        _worldService = worldService;
    }

    public IEnumerable<Animal> GetPotentialPrey(IWorldService worldService, Position position, double visionRadius)
    {
        return worldService.GetEntitiesInRange(position, visionRadius)
            .OfType<Herbivore>()
            .Where(h => !h.IsDead);
    }
}