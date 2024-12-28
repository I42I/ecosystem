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
    private readonly IEntityLocator<Animal> _entityLocator; //_animalLocator convient mieux
    private readonly IEntityLocator<Plant> _plantLocator;

    //private readonly Dictionary<Type, Func<double, double, Position, bool, Animal>> _animalFactories = new();
    //private readonly Dictionary<Type, Func<double, double, Position, Plant>> _plantFactories = new();
//Pourquoi ?
//Au lieu d’utiliser des conditions manuelles avec typeof(T), ces registres permettent d’enregistrer dynamiquement des fonctions pour créer des entités spécifiques.
//Cela élimine le besoin de modifier les méthodes CreateAnimal et CreatePlant chaque fois qu’un nouveau type est ajouté.
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


        //ci dessous rajouté
            // Enregistrement initial des types d'animaux et de plantes.
        // RegisterAnimal<Fox>((energy, health, position, isMale) => 
        //     new Fox(_animalLocator, _animalLocator, _worldService, _timeManager, position, (int)health, (int)energy, isMale));
        // RegisterAnimal<Rabbit>((energy, health, position, isMale) => 
        //     new Rabbit(_animalLocator, _plantLocator, _worldService, _timeManager, position, (int)health, (int)energy, isMale));

        // RegisterPlant<Grass>((energy, health, position) => 
        //     new Grass(_worldService, _timeManager, (int)health, (int)energy, position));
    }


//ajout de ces deux méthodes 
    // private void RegisterAnimal<T>(Func<double, double, Position, bool, T> factory) where T : Animal
    // {
    //     _animalFactories[typeof(T)] = (energy, health, position, isMale) => factory(energy, health, position, isMale);
    // }

    // private void RegisterPlant<T>(Func<double, double, Position, T> factory) where T : Plant
    // {
    //     _plantFactories[typeof(T)] = (energy, health, position) => factory(energy, health, position);
    // }



//Modification de ces 3 méthodes,elles sont adaptées en conséquences 
//Pourquoi ?
///->Ces changements utilisent les registres pour chercher dynamiquement une fonction de création au lieu d’évaluer manuellement chaque type avec typeof(T).
//Cela réduit le couplage direct avec des types spécifiques et facilite l’ajout de nouveaux types

    // public T CreateAnimal<T>(double energy, double health, Position position, bool isMale) where T : Animal
    // {
    //     // Recherche dans le registre dynamique.
    //     if (_animalFactories.TryGetValue(typeof(T), out var factory))
    //     {
    //         return (T)factory(energy, health, position, isMale);
    //     }

    //     // Si le type n'est pas supporté.
    //     throw new ArgumentException($"Unsupported animal type: {typeof(T).Name}");
    // }

    // public T CreatePlant<T>(double energy, double health, Position position) where T : Plant
    // {
    //     // Recherche dans le registre dynamique.
    //     if (_plantFactories.TryGetValue(typeof(T), out var factory))
    //     {
    //         return (T)factory(energy, health, position);
    //     }

    //     // Si le type n'est pas supporté.
    //     throw new ArgumentException($"Unsupported plant type: {typeof(T).Name}");
    // }

    // // Méthode pour créer de la viande (privée, appelée par d'autres services).
    // private Meat CreateMeat(Position position, int energyValue)
    // {
    //     return new Meat(position, energyValue, _timeManager);
    // }

    
    
    
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
        if (typeof(T) == typeof(Grass))
        {
            return (T)(Plant)new Grass(
                _worldService,
                _timeManager,
                (int)health,
                (int)energy,
                position);
        }
        
        throw new ArgumentException($"Unsupported plant type: {typeof(T).Name}");
    }

    private Meat CreateMeat(Position position, int energyValue)
    {
        return new Meat(position, energyValue, _timeManager);
    }
}
