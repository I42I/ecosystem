using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Behaviors.Base;
using ecosystem.Services.World;
using System.Linq;
using ecosystem.Services.Simulation;
using ecosystem.Helpers;

namespace ecosystem.Models.Behaviors.Reproduction;

public class MatingBehavior : IBehavior<Animal>
{
    private readonly IWorldService _worldService;
    private readonly ITimeManager _timeManager;
    public int Priority => 2; // Lower than fleeing, higher than eating
    
    private const double MATING_DURATION = 3.0; // seconds
    private double _matingTimer = 0;
    private Animal? _currentMate = null;

    public MatingBehavior(IWorldService worldService, ITimeManager timeManager)
    {
        _worldService = worldService;
        _timeManager = timeManager;
    }

    public bool CanExecute(Animal animal)
    {
        if (_matingTimer > 0 || animal.IsPregnant) return false;
        
        if (animal.IsMale)
        {
            var potentialMates = _worldService.GetEntitiesInRange(animal.Position, animal.VisionRadius)
                .OfType<Animal>()
                .Where(a => !a.IsMale 
                           && a.GetType() == animal.GetType() 
                           && !a.IsPregnant
                           && a.Energy > a.ReproductionEnergyThreshold);
                           
            return potentialMates.Any();
        }
        
        return false;
    }
    
    public void Execute(Animal animal)
    {
        if (_matingTimer > 0)
        {
            _matingTimer -= _timeManager.DeltaTime;
            if (_matingTimer <= 0)
            {
                FinishMating(animal);
            }
            return;
        }

        if (animal.IsMale)
        {
            var mate = FindMate(animal);
            if (mate != null && animal.IsInContactWith(mate))
            {
                StartMating(animal, mate);
            }
            else if (mate != null)
            {
                // Move towards mate
                var direction = mate.Position - animal.Position;
                animal.Move(direction.X, direction.Y);
            }
        }
    }

    private Animal? FindMate(Animal animal)
    {
        return _worldService.GetEntitiesInRange(animal.Position, animal.VisionRadius)
            .OfType<Animal>()
            .FirstOrDefault(a => a.GetType() == animal.GetType() 
                               && a.IsMale != animal.IsMale 
                               && !a.IsPregnant
                               && a.Energy >= a.ReproductionEnergyThreshold);
    }

    private void StartMating(Animal animal, Animal mate)
    {
        _matingTimer = MATING_DURATION;
        _currentMate = mate;
        mate.IsPregnant = true;
    }

    private void FinishMating(Animal animal)
    {
        _currentMate = null;
        // The pregnant female will handle birth through BirthBehavior
    }
}