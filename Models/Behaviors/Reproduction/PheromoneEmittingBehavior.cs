using System;
using ecosystem.Models.Behaviors.Base;
using ecosystem.Models.Entities.Animals;
using ecosystem.Services.World;

namespace ecosystem.Models.Behaviors.Reproduction;

public class PheromoneEmittingBehavior : IBehavior<Animal>
{
    private readonly IWorldService _worldService;
    
    public string Name => "EmittingPheromones";
    public int Priority => 1;

    public PheromoneEmittingBehavior(IWorldService worldService)
    {
        _worldService = worldService;
    }

    public bool CanExecute(Animal animal)
    {
        return !animal.IsMale && 
               !animal.IsPregnant && 
               animal.Energy >= animal.ReproductionEnergyThreshold;
    }

    public void Execute(Animal animal)
    {
        // Female just continues her normal behavior
        // Other behaviors will still execute as normal
        Console.WriteLine($"Female {animal.GetType().Name} is emitting pheromones");
    }
}