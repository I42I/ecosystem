using System.Collections.Generic;
using System.Linq;
using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Entities.Animals.Herbivores;
using ecosystem.Services.World;
using ecosystem.Helpers;
using ecosystem.Models.Core;
using ecosystem.Models.Entities.Environment;

namespace ecosystem.Models.Behaviors.Hunt;

public class WaterHuntingStrategy : IHuntingStrategy
{
    private readonly IWorldService _worldService;

    public WaterHuntingStrategy(IWorldService worldService)
    {
        _worldService = worldService;
    }

    public IEnumerable<Animal> GetPotentialPrey(IWorldService worldService, Position position, double visionRadius)
    {
        return worldService.GetEntitiesInRange(position, visionRadius)
            .OfType<Animal>()
            .Where(animal => 
                !animal.IsDead && 
                animal.PreferredEnvironment.HasFlag(EnvironmentType.Water) &&
                animal is Herbivore)
            .ToList();
    }
}