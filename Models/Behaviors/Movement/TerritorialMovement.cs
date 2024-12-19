using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Entities.Animals.Carnivores;
using ecosystem.Models.Behaviors.Base;
using ecosystem.Services.World;
using ecosystem.Helpers;
using ecosystem.Models.Core;
using System.Linq;

namespace ecosystem.Models.Behaviors.Movement;

public class TerritorialBehavior : IBehavior<Animal>
{
    private readonly IWorldService _worldService;
    private readonly double _territoryRadius;
    private readonly Position _territoryCenter;
    private readonly double _territoryOverlapThreshold;

    public TerritorialBehavior(
        IWorldService worldService, 
        Position spawnPosition,
        double territoryRadius = 100.0,
        double territoryOverlapThreshold = 50.0)
    {
        _worldService = worldService;
        _territoryCenter = spawnPosition;
        _territoryRadius = territoryRadius;
        _territoryOverlapThreshold = territoryOverlapThreshold;
    }

    public string Name => "Territorial";
    public int Priority => 2;

    public bool CanExecute(Animal animal)
    {
        // Vérifie si l'animal est trop loin de son territoire
        bool isTooFarFromTerritory = animal.GetDistanceTo(_territoryCenter) > _territoryRadius;
        
        // Vérifie si d'autres prédateurs sont trop proches
        bool isOtherPredatorNearby = CheckForNearbyPredators(animal);

        return isTooFarFromTerritory || isOtherPredatorNearby;
    }

    public void Execute(Animal animal)
    {
        var nearbyPredators = _worldService.GetEntitiesInRange(animal.Position, _territoryOverlapThreshold)
            .OfType<Carnivore>()
            .Where(p => p != animal && p.GetType() == animal.GetType());

        if (nearbyPredators.Any())
        {
            // S'éloigner des autres prédateurs
            var closestPredator = nearbyPredators.OrderBy(p => animal.GetDistanceTo(p.Position)).First();
            MoveAwayFromPredator(animal, closestPredator);
        }
        else
        {
            // Retourner vers le centre du territoire
            MoveTowardTerritory(animal);
        }
    }

    private bool CheckForNearbyPredators(Animal animal)
    {
        return _worldService.GetEntitiesInRange(animal.Position, _territoryOverlapThreshold)
            .OfType<Carnivore>()
            .Any(p => p != animal && p.GetType() == animal.GetType());
    }

    private void MoveAwayFromPredator(Animal animal, Animal predator)
    {
        var direction = animal.Position - predator.Position;
        var distance = MathHelper.CalculateDistance(animal.Position, predator.Position);
        
        if (distance > 0)
        {
            animal.Move(direction.X / distance, direction.Y / distance);
        }
    }

    private void MoveTowardTerritory(Animal animal)
    {
        var direction = _territoryCenter - animal.Position;
        var distance = MathHelper.CalculateDistance(animal.Position, _territoryCenter);
        
        if (distance > 0)
        {
            animal.Move(direction.X / distance, direction.Y / distance);
        }
    }
}