using System.Collections.Generic;
using ecosystem.Models.Core;
using ecosystem.Models.Entities.Animals;

namespace ecosystem.Models.Behaviors.Survival;

public interface IFleeingEntity
{
    double VisionRadius { get; }
    void FleeFromPredator(Animal predator);
    Position Position { get; }
}