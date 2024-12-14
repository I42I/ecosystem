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
            
            for (int i = 0; i < 1; i++)
            {
                var foxPosition = RandomHelper.GetRandomPosition(_worldService.Grid.Width, _worldService.Grid.Height);
                var fox = _entityFactory.CreateAnimal<Fox>(100, 100, foxPosition, i % 2 == 0);
                _worldService.AddEntity(fox);
            }

            for (int i = 0; i < 1; i++)
            {
                var rabbitPosition = RandomHelper.GetRandomPosition(_worldService.Grid.Width, _worldService.Grid.Height);
                var rabbit = _entityFactory.CreateAnimal<Rabbit>(80, 80, rabbitPosition, i % 2 == 0);
                _worldService.AddEntity(rabbit);
            }

            for (int i = 0; i < 1; i++)
            {
                var grassPosition = RandomHelper.GetRandomPosition(_worldService.Grid.Width, _worldService.Grid.Height);
                var grass = _entityFactory.CreatePlant<Grass>(50, 50, grassPosition);
                _worldService.AddEntity(grass);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating entities: {ex}");
        }
    }

    public void UpdateSimulation()
    {
        foreach (var entity in _worldService.Entities.ToList())
        {
            entity.Update();
        }
        SimulationUpdated?.Invoke(this, EventArgs.Empty);
    }
}