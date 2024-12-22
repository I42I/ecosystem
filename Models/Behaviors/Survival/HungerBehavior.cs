using System;
using ecosystem.Helpers;
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
        
        var hasLowEnergy = animal.Energy < herbivore.BaseHungerThreshold;
        Console.WriteLine($"[{animal.GetType().Name}#{animal.TypeId}] hunger check: Energy={animal.Energy}, Threshold={herbivore.BaseHungerThreshold}, Plant found={plant != null}");
        
        return hasLowEnergy && plant != null;
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