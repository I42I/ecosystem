using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Behaviors.Base;

namespace ecosystem.Models.Behaviors.Reproduction;

public class MatingBehavior : IBehavior<Animal>
{
    public int Priority => 1;
    
    public bool CanExecute(Animal animal)
    {
        return animal.NeedsToReproduce();
    }
    
    public void Execute(Animal animal)
    {
        var mate = animal.FindNearestMate();
        if (mate != null && animal.IsInContactWith(mate))
        {
            animal.Reproduce(mate);
        }
    }
}