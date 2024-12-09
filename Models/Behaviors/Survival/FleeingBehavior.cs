using System.Linq;
using ecosystem.Models.Core;
using ecosystem.Models.Entities.Animals.Herbivores;
using ecosystem.Models.Entities.Animals.Carnivores;
using ecosystem.Models.Behaviors.Base;

namespace ecosystem.Models.Behaviors.Survival;

public class FleeingBehavior : IBehavior
{
    public int Priority => 3;
    
    public bool CanExecute(LifeForm entity)
    {
        if (entity is Herbivore herbivore)
        {
            var nearbyPredators = herbivore._worldService.Entities
                .OfType<Carnivore>()
                .Where(c => herbivore.GetDistanceTo(c) <= herbivore.VisionRadius * 1.5);
            return nearbyPredators.Any();
        }
        return false;
    }
    
    public void Execute(LifeForm entity)
    {
        if (entity is Herbivore herbivore)
        {
            var predator = herbivore._worldService.Entities
                .OfType<Carnivore>()
                .Where(c => herbivore.GetDistanceTo(c) <= herbivore.VisionRadius * 1.5)
                .OrderBy(c => herbivore.GetDistanceTo(c))
                .First();
            
            herbivore.FleeFromPredator(predator);
        }
    }
}