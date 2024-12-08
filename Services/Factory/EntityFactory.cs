using System;
using System.Linq;
using ecosystem.Models.Entities.Animals.Carnivores;
using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Entities.Plants;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Behaviors;

namespace ecosystem.Services.Factory;

public class EntityFactory : IEntityFactory
{
    private readonly IEntityLocator<Animal> _animalLocator;
    private readonly IEntityLocator<Animal> _preyLocator;

    public EntityFactory(
        IEntityLocator<Animal> animalLocator,
        IEntityLocator<Animal> preyLocator)
    {
        _animalLocator = animalLocator;
        _preyLocator = preyLocator;
    }

    public T CreateAnimal<T>(double energy, double health, (double X, double Y) position, bool isMale) where T : Animal
    {
        Console.WriteLine($"Creating {typeof(T).Name} with position {position}");
        
        object[] parameters;
        if (typeof(T).IsSubclassOf(typeof(Carnivore)))
        {
            parameters = new object[] { _animalLocator, _preyLocator, (int)health, (int)energy, position, isMale };
        }
        else
        {
            parameters = new object[] { _animalLocator, (int)health, (int)energy, position, isMale };
        }

        var instance = Activator.CreateInstance(typeof(T), parameters) as T;

        if (instance == null)
        {
            throw new InvalidOperationException($"Failed to create instance of {typeof(T).Name}");
        }

        return instance;
    }

    public T CreatePlant<T>(double energy, double health, (double X, double Y) position) where T : Plant
    {
        var instance = Activator.CreateInstance(
            typeof(T),
            (int)health,
            (int)energy,
            position) as T;

        if (instance == null)
        {
            throw new InvalidOperationException($"Failed to create instance of {typeof(T).Name}");
        }

        return instance;
    }
}

