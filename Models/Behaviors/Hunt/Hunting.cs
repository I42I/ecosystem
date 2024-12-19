using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Behaviors.Base;
using ecosystem.Services.World;
using System.Linq;
using System;
using ecosystem.Helpers;
using ecosystem.Models.Entities.Animals.Carnivores;

namespace ecosystem.Models.Behaviors.Hunt;

public class HuntingBehavior : IBehavior<Animal>
{
    private readonly IWorldService _worldService;
    private readonly IHuntingStrategy _huntingStrategy;
    
    public HuntingBehavior(IWorldService worldService, IHuntingStrategy huntingStrategy)
    {
        _worldService = worldService;
        _huntingStrategy = huntingStrategy;
    }
    public string Name => "Hunt";
    public int Priority => 3;

    public bool CanExecute(Animal animal)
    {
        if (!(animal is IPredator predator)) return false;
        
        if (animal.Energy >= animal.HungerThreshold) return false;
        
        var prey = FindNearestPrey(animal);
        Console.WriteLine($"{animal.GetType().Name} hunting check: Energy={animal.Energy}, Threshold={animal.HungerThreshold}, PreyFound={prey != null}");
        
        return prey != null;
    }

    public void Execute(Animal animal)
    {
        if (!(animal is IPredator)) return;

        var prey = FindNearestPrey(animal);
        if (prey == null) return;

        if (MathHelper.IsInContactWith(animal, prey))
        {
            Attack(animal, prey);
        }
        else
        {
            MoveTowardsPrey(animal, prey);
        }
    }

    private Animal? FindNearestPrey(Animal predator)
    {
        var potentialPrey = _huntingStrategy.GetPotentialPrey(_worldService);
        return potentialPrey
            .OrderBy(prey => predator.GetDistanceTo(prey.Position))
            .FirstOrDefault();
    }

    private void Attack(Animal animal, Animal prey)
    {
        var predator = animal as Carnivore;
        if (predator == null) return;
        
        int damage = _huntingStrategy.CalculateAttackDamage(predator.BaseAttackPower);
        int energyGain = _huntingStrategy.CalculateEnergyGain(predator.BaseAttackPower);
        
        prey.TakeDamage(damage);
        predator.AddEnergy(energyGain);
    }

    private void MoveTowardsPrey(Animal predator, Animal prey)
    {
        var direction = prey.Position - predator.Position;
        var distance = predator.GetDistanceTo(prey.Position);
        
        if (distance > 0)
        {
            predator.Move(direction.X / distance, direction.Y / distance);
        }
    }
}