using System.Collections.Generic;
using System.Linq;
using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Entities.Animals.Herbivores;
using ecosystem.Services.World;
using ecosystem.Helpers;

namespace ecosystem.Models.Behaviors.Hunt;

public class GroundHuntingStrategy : IHuntingStrategy
{
    public int CalculateAttackDamage(double baseAttackPower)
    {
        return (int)(baseAttackPower * (0.8 + RandomHelper.Instance.NextDouble() * 0.4));
    }

    public int CalculateEnergyGain(double baseAttackPower)
    {
        return (int)(baseAttackPower * 0.5);
    }

    public IEnumerable<Animal> GetPotentialPrey(IWorldService worldService)
    {
        return worldService.Entities
            .OfType<Herbivore>()
            .Where(h => !h.IsDead);
    }
}