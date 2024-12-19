using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using ecosystem.Helpers;
using ecosystem.Models.Core;
using ecosystem.Models.Behaviors;
using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Entities.Animals.Carnivores;
using ecosystem.Models.Entities.Animals.Herbivores;
using ecosystem.Models.Entities.Plants;
using ecosystem.Services.World;
using ecosystem.Services.Factory;

namespace ecosystem.Services.Simulation;

public interface ISimulationEngine
{
    event EventHandler? SimulationUpdated;
    void InitializeSimulation();
    void CreateInitialEntities();
    void UpdateSimulation();
}

public class SimulationEngine : ISimulationEngine
{
    public event EventHandler? SimulationUpdated;

    private readonly IWorldService _worldService;
    private readonly IEntityFactory _entityFactory;
    private readonly ITimeManager _timeManager;
    private readonly object _updateLock = new object();

    public SimulationEngine(
        IWorldService worldService,
        IEntityFactory entityFactory,
        ITimeManager timeManager)
    {
        _worldService = worldService;
        _entityFactory = entityFactory;
        _timeManager = timeManager;

        _timeManager.RegisterTickAction(UpdateSimulation);
    }

    public void InitializeSimulation()
    {
        try
        {
            CreateInitialEntities();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to initialize simulation", ex);
        }
    }

    public void CreateInitialEntities()
    {
        try
        {
            Console.WriteLine("Creating initial entities...");
            
            _worldService.Entities.Clear();
            
            CreateAnimal<Fox>(3, 100, 100);
            CreateAnimal<Rabbit>(5, 80, 80);
            CreatePlants<Grass>(10, 50, 50);
            
            Console.WriteLine($"Entities created: {_worldService.Entities.Count}");
            _worldService.ProcessEntityQueues();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating entities: {ex}");
            throw;
        }
    }

    private void CreateAnimal<T>(int count, double health, double energy) where T : Animal
    {
        for (int i = 0; i < count; i++)
        {
            var position = RandomHelper.GetRandomPosition();
            var animal = _entityFactory.CreateAnimal<T>(energy, health, position, i % 2 == 0);
            _worldService.AddEntity(animal);
        }
    }

    private void CreatePlants<T>(int count, double health, double energy) where T : Plant
    {
        for (int i = 0; i < count; i++)
        {
            var position = RandomHelper.GetRandomPosition();
            var plant = _entityFactory.CreatePlant<T>(energy, health, position);
            _worldService.AddEntity(plant);
        }
    }

    public void UpdateSimulation()
    {
        lock (_updateLock)
        {
            var entities = _worldService.Entities.ToList();
            foreach (var entity in entities)
            {
                try
                {
                    entity.Update();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating entity: {ex.Message}");
                }
            }
            
            _worldService.ProcessEntityQueues();
            
            Dispatcher.UIThread.Post(() => 
            {
                SimulationUpdated?.Invoke(this, EventArgs.Empty);
            });
        }
    }
}