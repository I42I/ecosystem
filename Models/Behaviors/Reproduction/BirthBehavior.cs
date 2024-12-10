using ecosystem.Models.Core;
using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Behaviors.Base;
using ecosystem.Helpers;

namespace ecosystem.Models.Behaviors.Reproduction;

public class BirthBehavior : IBehavior<Animal>
{
    public int Priority => 4;
    
    public bool CanExecute(Animal animal)
    {
        return animal.IsPregnant;
    }
    
    public void Execute(Animal animal)
    {
        var (x, y) = RandomHelper.GetRandomPositionInRadius(animal.Position.X, animal.Position.Y, 2.0);
        var offspring = animal.CreateOffspring(new Position(x, y));
        animal.WorldService.AddEntity(offspring);
        animal.IsPregnant = false;
    }
}