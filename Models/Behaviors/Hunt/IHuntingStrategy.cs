using System.Collections.Generic;
using ecosystem.Models.Entities.Animals;
using ecosystem.Services.World;
using ecosystem.Models.Core;

namespace ecosystem.Models.Behaviors.Hunt;

public interface IHuntingStrategy
{
    IEnumerable<Animal> GetPotentialPrey(IWorldService worldService, Position position, double visionRadius);
}