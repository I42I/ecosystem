using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Behaviors.Base;
using ecosystem.Models.Entities.Animals.Herbivores;
using ecosystem.Helpers;

namespace ecosystem.Models.Behaviors.Survival;

public class HungerBehavior : IBehavior<Animal>
{
    public string Name => "Hunger";
    public int Priority => 2;

    public bool CanExecute(Animal animal)
    {
        if (!(animal is Herbivore herbivore))
            return false;

        return animal.Energy < animal.HungerThreshold;
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