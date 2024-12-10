using System.Linq;
using ecosystem.Models.Core;
using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Entities.Animals.Carnivores;
using ecosystem.Models.Behaviors.Base;


namespace ecosystem.Models.Behaviors.Survival;

public class FleeingBehavior : IBehavior<Animal>
{
    public int Priority => 3;
    
    public bool CanExecute(Animal animal)
    {
        return animal is IFleeingEntity fleeingEntity && 
               fleeingEntity.GetNearbyEntities(fleeingEntity.VisionRadius * 1.5)
                          .OfType<Carnivore>()
                          .Any();
    }
    
    public void Execute(Animal animal)
    {
        if (animal is IFleeingEntity fleeingEntity)
        {
            var predator = fleeingEntity.GetNearbyEntities(fleeingEntity.VisionRadius * 1.5)
                .OfType<Carnivore>()
                .OrderBy(c => animal.GetDistanceTo(c.Position))
                .First();

            fleeingEntity.FleeFromPredator(predator);
        }
    }
}