using System;
using ecosystem.Helpers;
using ecosystem.Services.Simulation;
using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Behaviors.Base;
using ecosystem.Models.Entities.Animals.Herbivores;

namespace ecosystem.Models.Behaviors.Survival;

public class HungerBehavior : IBehavior<Animal>
{
    public string Name => "Hunger";
    public int Priority => 2;

    public bool CanExecute(Animal animal)
    {
        if (!(animal is Herbivore herbivore))
            return false;

        var plant = herbivore.FindNearestPlant();
        
        var shouldEat = animal.Energy <= herbivore.BaseHungerThreshold ||
                        animal.HealthPoints < animal.MaxHealth ||
                       (animal.Energy < 0.95 * animal.MaxEnergy && 
                        plant != null && 
                        MathHelper.IsInContactWith(animal, plant));
                       
        return shouldEat && plant != null;
    }

    public void Execute(Animal animal)
    {
        if (animal is Herbivore herbivore)
        {
            var plant = herbivore.FindNearestPlant();
            if (plant != null)
            {
                if (MathHelper.IsInContactWith(herbivore, plant))
                {
                    herbivore.Eat(plant);
                    
                    if (herbivore.Energy >= SimulationConstants.HEALING_ENERGY_THRESHOLD)
                    {
                        herbivore.ConvertEnergyToHealth(herbivore.Energy - SimulationConstants.HEALING_ENERGY_THRESHOLD);
                    }
                }
                else
                {
                    var direction = plant.Position - herbivore.Position;
                    var distance = herbivore.GetDistanceTo(plant.Position);
                    
                    if (distance > 0)
                    {
                        herbivore.Move(direction.X / distance, direction.Y / distance);
                    }
                }
            }
        }
    }
}