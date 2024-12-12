using ecosystem.Models.Behaviors.Base;
using ecosystem.Models.Entities.Animals;
using System.Collections.Generic;

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
    
    protected Animal? FindNearestMate()
    {
        return _entityLocator.FindNearest(
            GetPotentialMates(),
            VisionRadius
        );
    }

    private IEnumerable<Animal> GetPotentialMates()
    {
        return new List<Animal>();
    }
}