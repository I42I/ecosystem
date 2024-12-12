using System;
using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Entities.Plants;
using ecosystem.Models.Core;
using Microsoft.Extensions.DependencyInjection;

namespace ecosystem.Services.Factory;

public interface IEntityFactory
{
    T CreateAnimal<T>(double energy, double health, Position position, bool isMale) where T : Animal;
    T CreatePlant<T>(double energy, double health, Position position) where T : Plant;
}

public class EntityFactory : IEntityFactory
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T CreateAnimal<T>(double energy, double health, Position position, bool isMale) where T : Animal
    {
        return ActivatorUtilities.CreateInstance<T>(_serviceProvider, energy, health, position, isMale);
    }

    public T CreatePlant<T>(double energy, double health, Position position) where T : Plant
    {
        return ActivatorUtilities.CreateInstance<T>(_serviceProvider, energy, health, position);
    }
}
