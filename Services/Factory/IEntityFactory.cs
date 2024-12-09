using ecosystem.Models;
using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Entities.Plants;
using ecosystem.Models.Core;

namespace ecosystem.Services.Factory;

public interface IEntityFactory
{
    T CreateAnimal<T>(double energy, double health, Position position, bool isMale) where T : Animal;
    T CreatePlant<T>(double energy, double health, Position position) where T : Plant;
}
