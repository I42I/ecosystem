using ecosystem.Models.Core;
using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Behaviors.Base;

namespace ecosystem.Models.Behaviors.Survival;

public class HungerBehavior : IBehavior
{
    public int Priority => 2;
    
    public bool CanExecute(LifeForm entity)
    {
        return entity is Animal animal && animal.Energy <= animal.HungerThreshold;
    }
    
    public void Execute(LifeForm entity)
    {
        if (entity is Animal animal)
        {
            animal.SearchForFood();
        }
    }
}