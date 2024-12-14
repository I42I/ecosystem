using System.Collections.Generic;
using ecosystem.Models.Entities.Animals;
using ecosystem.Services.World;

namespace ecosystem.Models.Behaviors.Hunt;

public interface IHuntingStrategy
{
    int CalculateAttackDamage(double baseAttackPower);
    int CalculateEnergyGain(double baseAttackPower);
    IEnumerable<Animal> GetPotentialPrey(IWorldService worldService);
}