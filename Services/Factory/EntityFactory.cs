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
using ecosystem.Models.Entities.Environment;
using ecosystem.Helpers;

namespace ecosystem.Services.Factory;

public interface IEntityFactory
{
    T CreateAnimal<T>(double initialHealthPercent, double initialEnergyPercent, Position position, bool isMale) where T : Animal;
    T CreatePlant<T>(double initialHealthPercent, double initialEnergyPercent, Position position) where T : Plant;
    Meat CreateMeat(Position position);
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
    
    public T CreateAnimal<T>(double initialHealthPercent, double initialEnergyPercent, Position position, bool isMale) where T : Animal
    {
        initialHealthPercent = Math.Clamp(initialHealthPercent, 0, 100);
        initialEnergyPercent = Math.Clamp(initialEnergyPercent, 0, 100);

        if (typeof(T) == typeof(Fox))
        {
            if (!_worldService.IsValidSpawnLocation(position, Fox.DefaultEnvironment))
            {
                position = RandomHelper.GetRandomPositionForEnvironment(Fox.DefaultEnvironment,  _worldService);
            }

            var fox = new Fox(
                _entityLocator,
                _entityLocator,
                _worldService,
                _timeManager,
                this,
                position,
                (int)(Fox.DefaultMaxHealth * initialHealthPercent / 100),
                (int)(Fox.DefaultMaxEnergy * initialEnergyPercent / 100),
                isMale);
            return (T)(Animal)fox;
        }
        else if (typeof(T) == typeof(Rabbit))
        {
            if (!_worldService.IsValidSpawnLocation(position, Rabbit.DefaultEnvironment))
            {
                position = RandomHelper.GetRandomPositionForEnvironment(Rabbit.DefaultEnvironment,  _worldService);
            }

            var rabbit = new Rabbit(
                _entityLocator,
                _plantLocator,
                _worldService,
                _timeManager,
                this,
                position,
                (int)(Rabbit.DefaultMaxHealth * initialHealthPercent / 100),
                (int)(Rabbit.DefaultMaxEnergy * initialEnergyPercent / 100),
                isMale);
            return (T)(Animal)rabbit;
        }
        else if (typeof(T) == typeof(Squirrel))
        {
            if (!_worldService.IsValidSpawnLocation(position, Squirrel.DefaultEnvironment))
            {
                position = RandomHelper.GetRandomPositionForEnvironment(Squirrel.DefaultEnvironment,  _worldService);
            }

            var squirrel = new Squirrel(
                _entityLocator,
                _plantLocator,
                _worldService,
                _timeManager,
                this,
                position,
                (int)(Squirrel.DefaultMaxHealth * initialHealthPercent / 100),
                (int)(Squirrel.DefaultMaxEnergy * initialEnergyPercent / 100),
                isMale);
            return (T)(Animal)squirrel;
        }

        throw new ArgumentException($"Unsupported animal type: {typeof(T).Name}");
    }

    public T CreatePlant<T>(double initialHealthPercent, double initialEnergyPercent, Position position) where T : Plant
    {
        initialHealthPercent = Math.Clamp(initialHealthPercent, 0, 100);
        initialEnergyPercent = Math.Clamp(initialEnergyPercent, 0, 100);

        if (typeof(T) == typeof(Grass))
        {
            if (!_worldService.IsValidSpawnLocation(position, Grass.DefaultEnvironment))
            {
                position = RandomHelper.GetRandomPositionForEnvironment(
                    Grass.DefaultEnvironment,
                    _worldService
                );
            }

            var grass = new Grass(
                _worldService,
                _timeManager,
                this,
                (int)(Grass.DefaultMaxHealth * initialHealthPercent / 100),
                (int)(Grass.DefaultMaxEnergy * initialEnergyPercent / 100),
                position
            );
            return (T)(Plant)grass;
        }
        else if (typeof(T) == typeof(Algae))
        {
            if (!_worldService.IsValidSpawnLocation(position, Algae.DefaultEnvironment))
            {
                position = RandomHelper.GetRandomPositionForEnvironment(
                    Algae.DefaultEnvironment,
                    _worldService
                );
            }

            var algae = new Algae(
                _worldService,
                _timeManager,
                this,
                (int)(Algae.DefaultMaxHealth * initialHealthPercent / 100),
                (int)(Algae.DefaultMaxEnergy * initialEnergyPercent / 100),
                position
            );
            return (T)(Plant)algae;
        }

        throw new ArgumentException($"Unsupported plant type: {typeof(T).Name}");
    }

    public Meat CreateMeat(Position position)
    {
        return new Meat(
            position,
            Meat.DefaultMaxHealth,
            Meat.DefaultMaxEnergy,
            _timeManager,
            _worldService
        );
    }
}
