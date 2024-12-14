using System;
using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Entities.Plants;
using ecosystem.Models.Core;
using Microsoft.Extensions.DependencyInjection;
using ecosystem.Services.World;
using ecosystem.Services.Simulation;
using ecosystem.Models.Behaviors;
using ecosystem.Models.Entities.Animals.Carnivores;
using ecosystem.Models.Entities.Animals.Herbivores;

namespace ecosystem.Services.Factory;

public interface IEntityFactory
{
    T CreateAnimal<T>(double energy, double health, Position position, bool isMale) where T : Animal;
    T CreatePlant<T>(double energy, double health, Position position) where T : Plant;
}

public class EntityFactory : IEntityFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IWorldService _worldService;
    private readonly ITimeManager _timeManager;
    private readonly IEntityLocator<Animal> _entityLocator;
    private readonly IEntityLocator<Plant> _plantLocator;

    public EntityFactory(
        IServiceProvider serviceProvider,
        IWorldService worldService,
        ITimeManager timeManager,
        IEntityLocator<Animal> entityLocator,
        IEntityLocator<Plant> plantLocator)
    {
        _serviceProvider = serviceProvider;
        _worldService = worldService;
        _timeManager = timeManager;
        _entityLocator = entityLocator;
        _plantLocator = plantLocator;
    }
    public T CreateAnimal<T>(double energy, double health, Position position, bool isMale) where T : Animal
    {
        if (typeof(T) == typeof(Fox))
        {
            return (T)(Animal)new Fox(
                _entityLocator,
                _entityLocator,
                _worldService,
                _timeManager,
                position,
                (int)health,
                (int)energy,
                isMale);
        }
        else if (typeof(T) == typeof(Rabbit))
        {
            return (T)(Animal)new Rabbit(
                _entityLocator,
                _plantLocator,
                _worldService,
                _timeManager,
                position,
                (int)health,
                (int)energy,
                isMale);
        }
        // ... handle other animal types

        throw new ArgumentException($"Unsupported animal type: {typeof(T).Name}");
    }

    public T CreatePlant<T>(double energy, double health, Position position) where T : Plant
    {
        return ActivatorUtilities.CreateInstance<T>(
            _serviceProvider,
            _worldService,
            (int)health,
            (int)energy,
            position);
    }
}
