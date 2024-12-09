using System;
using System.Linq;
using ecosystem.Models.Entities.Animals.Carnivores;
using ecosystem.Models.Entities.Animals.Herbivores;
using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Entities.Plants;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Behaviors;
using ecosystem.Services.World;
using ecosystem.Models.Core;

namespace ecosystem.Services.Factory;

public class EntityFactory : IEntityFactory
{
    private readonly IEntityLocator<Animal> _animalLocator;
    private readonly IEntityLocator<Animal> _preyLocator;
    private readonly IEntityLocator<Plant> _plantLocator;
    private readonly IWorldService _worldService;

    public EntityFactory(
        IEntityLocator<Animal> animalLocator,
        IEntityLocator<Animal> preyLocator,
        IEntityLocator<Plant> plantLocator,
        IWorldService worldService)
    {
        _animalLocator = animalLocator;
        _preyLocator = preyLocator;
        _plantLocator = plantLocator;
        _worldService = worldService;
    }

    public T CreateAnimal<T>(double energy, double health, Position position, bool isMale) where T : Animal
    {
        if (typeof(T) == typeof(Fox))
        {
            return new Fox(_animalLocator, _preyLocator, _worldService, (int)health, (int)energy, position, isMale) as T 
                ?? throw new InvalidOperationException($"Failed to create {typeof(T).Name}");
        }
        else if (typeof(T) == typeof(Rabbit))
        {
            return new Rabbit(_animalLocator, _plantLocator, _worldService, (int)health, (int)energy, position, isMale) as T
                ?? throw new InvalidOperationException($"Failed to create {typeof(T).Name}");
        }
        
        throw new NotSupportedException($"Creation of {typeof(T).Name} is not supported");
    }

    public T CreatePlant<T>(double energy, double health, Position position) where T : Plant
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

