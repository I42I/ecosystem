using ecosystem.Models.Behaviors.Base;
using ecosystem.Models.Entities.Animals;

namespace ecosystem.Models.Behaviors.Reproduction;

public class ReproductionBehavior : IBehavior<Animal>
{
    public int Priority => 1;

    public bool CanExecute(Animal animal)
    {
        return animal is IMating matingEntity &&
               animal.Energy >= matingEntity.ReproductionEnergyThreshold;
    }

    public void Execute(Animal animal)
    {
        if (animal is IMating matingEntity)
        {
            var mate = FindMate(animal);
            if (mate != null)
            {
                var offspring = matingEntity.CreateOffspring(animal.Position);
                animal.WorldService.AddEntity(offspring);
                animal.Energy -= matingEntity.ReproductionEnergyThreshold;
            }
        }
    }

    private Animal? FindMate(Animal animal)
    {
        // Implement logic to find a mate
        return null; // Placeholder
    }
}