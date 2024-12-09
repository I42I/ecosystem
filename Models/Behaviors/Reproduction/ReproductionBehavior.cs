using ecosystem.Models.Behaviors.Base;
using ecosystem.Models.Core;
using ecosystem.Models.Entities.Animals;

namespace ecosystem.Models.Behaviors.Reproduction;

public class ReproductionBehavior : IBehavior
{
    public int Priority => 1;
    
    public bool CanExecute(LifeForm entity)
    {
        return entity is Animal animal && animal.NeedsToReproduce();
    }
    
    public void Execute(LifeForm entity)
    {
        if (entity is Animal animal)
        {
            animal.SearchForMate();
        }
    }
}