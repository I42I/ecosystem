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
                    Console.WriteLine($"[{herbivore.GetType().Name}#{herbivore.TypeId}] Distance to plant: {herbivore.GetDistanceTo(plant.Position):F3}");
                    herbivore.Eat(plant);
                }
                else
                {
                    Console.WriteLine($"[{herbivore.GetType().Name}#{herbivore.TypeId}] Moving towards plant. Distance: {herbivore.GetDistanceTo(plant.Position):F3}");

                    var direction = plant.Position - herbivore.Position;
                    var distance = herbivore.GetDistanceTo(plant.Position);

                    // Debug output
                    Console.WriteLine($"[{herbivore.GetType().Name}#{herbivore.TypeId}] Plant position: ({plant.Position.X:F3}, {plant.Position.Y:F3})");
                    Console.WriteLine($"[{herbivore.GetType().Name}#{herbivore.TypeId}] Rabbit position: ({herbivore.Position.X:F3}, {herbivore.Position.Y:F3})");
                    Console.WriteLine($"[{herbivore.GetType().Name}#{herbivore.TypeId}] Direction vector: ({direction.X:F3}, {direction.Y:F3})");
                    
                    if (distance > 0)
                    {
                        Console.WriteLine($"[{herbivore.GetType().Name}#{herbivore.TypeId}] Moving towards plant: direction.X : {direction.X}, dx={direction.X / distance:F3} / direction.Y : {direction.Y}, dy={direction.Y / distance:F3}");
                        herbivore.Move(direction.X / distance, direction.Y / distance);
                    }
                }
            }
        }
    }
}