using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Core;

namespace ecosystem.Models.Behaviors.Base;

public class RestBehavior : IBehavior
{
    public int Priority => 0;
    
    public bool CanExecute(LifeForm entity)
    {
        return entity is Animal;
    }
    
    public void Execute(LifeForm entity)
    {
        if (entity is Animal animal)
        {
            animal.Rest();
        }
    }
}