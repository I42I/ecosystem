using System;
using ecosystem.Models.Behaviors.Base;
using ecosystem.Models.Entities.Animals;
using ecosystem.Services.World;

namespace ecosystem.Models.Behaviors.Reproduction;

public class PheromoneEmittingBehavior : IBehavior<Animal>
{
    private readonly IWorldService _worldService;
    private readonly RestBehavior _restBehavior;
    
    public string Name => "EmittingPheromones";
    public int Priority => 1;

    public PheromoneEmittingBehavior(IWorldService worldService)
    {
        _worldService = worldService;
        _restBehavior = new RestBehavior();
    }

    public bool CanExecute(Animal animal)
    {
        return !animal.IsMale && 
               !animal.IsPregnant && 
               animal.Energy >= animal.ReproductionEnergyThreshold;
    }

    public void Execute(Animal animal)
    {
        _restBehavior.Execute(animal);
    }
}