using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Behaviors.Base;

namespace ecosystem.Models.Behaviors.Survival;

public class HungerBehavior : IBehavior<Animal>
{
    public int Priority => 2;

    public bool CanExecute(Animal animal)
    {
        return animal is IHungry hungryEntity &&
               hungryEntity.HungerThreshold >= animal.Energy;
    }

    public void Execute(Animal animal)
    {
        if (animal is IHungry hungryEntity)
        {
            hungryEntity.SearchForFood();
        }
    }
}