using System;
using System.Linq;
using ecosystem.Models.Behaviors.Base;
using ecosystem.Models.Entities.Animals;
using ecosystem.Services.World;
using ecosystem.Helpers;
using ecosystem.Services.Simulation;

namespace ecosystem.Models.Behaviors.Reproduction;

public class PheromoneAttractedBehavior : IBehavior<Animal>
{
    private readonly IWorldService _worldService;
    private const double MATING_SPEED_MULTIPLIER = 1.2;

    public string Name => "AttractedToPheromones";
    public int Priority => 2;

    public PheromoneAttractedBehavior(IWorldService worldService)
    {
        _worldService = worldService;
    }

    public bool CanExecute(Animal animal)
    {
        if (!animal.IsMale || 
            animal.Energy < animal.ReproductionEnergyThreshold ||
            animal.ReproductionCooldown > 0)
            return false;

        var potentialMates = _worldService.GetEntitiesInRange(animal.Position, animal.VisionRadius)
            .OfType<Animal>()
            .Where(a => !a.IsMale 
                       && a.GetType() == animal.GetType() 
                       && !a.IsPregnant
                       && a.ReproductionCooldown <= 0
                       && a.Energy >= a.ReproductionEnergyThreshold);

        return potentialMates.Any();
    }

    public void Execute(Animal animal)
    {
        var mate = FindMate(animal);
        if (mate != null)
        {
            if (animal.IsInContactWith(mate))
            {
                // Initiate reproduction
                animal.RemoveEnergy((int)animal.ReproductionEnergyCost);
                mate.IsPregnant = true;
                animal.ReproductionCooldown = SimulationConstants.MALE_REPRODUCTION_COOLDOWN;
                mate.ReproductionCooldown = SimulationConstants.GESTATION_PERIOD;
                Console.WriteLine($"Male {animal.GetType().Name} successfully mated");
            }
            else
            {
                // Move towards mate with increased speed
                var direction = mate.Position - animal.Position;
                var originalSpeed = animal.MovementSpeed;
                animal.MovementSpeed *= MATING_SPEED_MULTIPLIER;
                animal.Move(direction.X, direction.Y);
                animal.MovementSpeed = originalSpeed;
            }
        }
    }

    private Animal? FindMate(Animal animal)
    {
        return _worldService.GetEntitiesInRange(animal.Position, animal.VisionRadius)
            .OfType<Animal>()
            .Where(a => !a.IsMale 
                       && a.GetType() == animal.GetType() 
                       && !a.IsPregnant
                       && a.Energy >= a.ReproductionEnergyThreshold)
            .OrderBy(a => animal.GetDistanceTo(a.Position))
            .FirstOrDefault();
    }
}