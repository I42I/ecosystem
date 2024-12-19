using System.Linq;
using ecosystem.Models.Behaviors.Base;
using ecosystem.Models.Entities.Animals;
using ecosystem.Services.World;
using ecosystem.Helpers;

namespace ecosystem.Models.Behaviors.Reproduction;

public class ReproductionBehavior : IBehavior<Animal>
{
    private readonly IWorldService _worldService;

    public ReproductionBehavior(IWorldService worldService)
    {
        _worldService = worldService;
    }

    public string Name => "Reproduction";
    public int Priority => 1;

    public bool CanExecute(Animal animal)
    {
        return !animal.IsPregnant && animal.Energy >= animal.ReproductionEnergyThreshold;
    }

    public void Execute(Animal animal)
    {
        var potentialMates = _worldService.GetEntitiesInRange(animal.Position, animal.VisionRadius)
            .OfType<Animal>()
            .Where(a => a.GetType() == animal.GetType() 
                       && a.IsMale != animal.IsMale
                       && !a.IsPregnant
                       && a.Energy >= a.ReproductionEnergyThreshold);

        var mate = potentialMates.FirstOrDefault();
        if (mate != null && MathHelper.IsInContactWith(animal, mate))
        {
            animal.RemoveEnergy((int)animal.ReproductionEnergyCost);
            if (!animal.IsMale)
            {
                animal.IsPregnant = true;
            }
        }
    }
}