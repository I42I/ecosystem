using System;
using ecosystem.Models.Core;
using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Behaviors.Base;
using ecosystem.Helpers;
using ecosystem.Services.Simulation;

namespace ecosystem.Models.Behaviors.Reproduction;

public class BirthBehavior : IBehavior<Animal>
{
    public string Name => "Birth";
    public int Priority => 4;
    
    public bool CanExecute(Animal animal)
    {
        return !animal.IsMale && 
               animal.IsPregnant && 
               animal.ReproductionCooldown <= 0 &&
               animal.IsReadyToGiveBirth();
    }
    
    public void Execute(Animal animal)
    {
        var spawnPosition = RandomHelper.GetRandomPositionInRadiusForEnvironment(
            animal.Position.X,
            animal.Position.Y,
            animal.ContactRadius * 2,
            animal.PreferredEnvironment,
            animal.WorldService
        );
            
        var offspring = animal.CreateOffspring(spawnPosition);
        animal.WorldService.AddEntity(offspring);
        
        animal.IsPregnant = false;
        animal.ReproductionCooldown = SimulationConstants.FEMALE_REPRODUCTION_COOLDOWN;
        
        Console.WriteLine($"Female {animal.GetType().Name} gave birth at ({spawnPosition.X:F2}, {spawnPosition.Y:F2})");
    }
}