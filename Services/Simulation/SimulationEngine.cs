using System;
using System.Linq;
using System.Collections.Generic;
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
    void ResetSimulation();
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
            RandomHelper.Initialize(42);
            CreateInitialEntities();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to initialize simulation", ex);
        }
    }

    public void ResetSimulation()
    {
        try
        {
            lock (_updateLock)
            {
                _timeManager.Pause();
                _timeManager.WaitForStop(); 

                foreach (var entity in _worldService.Entities.ToList())
                {
                    _worldService.RemoveEntity(entity);
                }
                _worldService.ProcessEntityQueues();

                RandomHelper.Initialize(42);

                CreateInitialEntities();

                _timeManager.Reset();
                _timeManager.Start();
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to reset simulation", ex);
        }
    }

    public void CreateInitialEntities()
    {
        try
        {
            Console.WriteLine("Creating initial entities...");
            
            foreach (var entity in _worldService.Entities.ToList())
            {
                _worldService.RemoveEntity(entity);
            }
            _worldService.ProcessEntityQueues();
            
            var entities = new List<Entity>();
            
            for (int i = 0; i < 3; i++)
            {
                var position = RandomHelper.GetRandomPosition();
                var fox = _entityFactory.CreateAnimal<Fox>(100, 100, position, i % 2 == 0);
                entities.Add(fox);
            }
            
            for (int i = 0; i < 10; i++)
            {
                var position = RandomHelper.GetRandomPosition();
                var squirrel = _entityFactory.CreateAnimal<Squirrel>(100, 100, position, i % 2 == 0);
                entities.Add(squirrel);
            }
            
            for (int i = 0; i < 20; i++)
            {
                var position = RandomHelper.GetRandomPosition();
                var grass = _entityFactory.CreatePlant<Grass>(100, 100, position);
                entities.Add(grass);
            }

            foreach (var entity in entities)
            {
                _worldService.AddEntity(entity);
            }
            
            _worldService.ProcessEntityQueues();
            
            Console.WriteLine($"Created entities: {entities.Count}");
            Console.WriteLine($"World entities count: {_worldService.Entities.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating entities: {ex}");
            throw;
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