using ecosystem.Models;
using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Entities.Plants;

namespace ecosystem.Services.Factory;

public interface IEntityFactory
{
    T CreateAnimal<T>(double energy, double health, (double X, double Y) position, bool isMale) where T : Animal;
    T CreatePlant<T>(double energy, double health, (double X, double Y) position) where T : Plant;
}
