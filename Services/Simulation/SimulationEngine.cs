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
    double SimulationSpeed { get; set; }
    void Start();
    void Pause();
    void Reset();
    void InitializeSimulation();
    void CreateInitialEntities();
}

public class SimulationEngine : ISimulationEngine
{
    public event EventHandler? SimulationUpdated;
    private bool _isRunning;
    private Timer? _timer;
    private double _simulationSpeed = 1.0;

    private readonly IWorldService _worldService;
    private readonly IEntityFactory _entityFactory;
    private readonly Random _random;
    private readonly IEntityLocator<Animal> _animalLocator;
    private readonly IEntityLocator<Animal> _preyLocator;

    public SimulationEngine(
        IWorldService worldService, 
        IEntityFactory entityFactory,
        IEntityLocator<Animal> animalLocator,
        IEntityLocator<Animal> preyLocator)
    {
        _worldService = worldService;
        _entityFactory = entityFactory;
        _animalLocator = animalLocator;
        _preyLocator = preyLocator;
        _random = RandomHelper.Instance;
    }

    public double SimulationSpeed
    {
        get => _simulationSpeed;
        set
        {
            _simulationSpeed = value;
            UpdateTimer();
        }
    }

    public void Start()
    {
        _isRunning = true;
        UpdateTimer();
    }

    public void Pause()
    {
        _isRunning = false;
        _timer?.Dispose();
    }

    public void Reset()
    {
        Pause();
        InitializeSimulation();
    }

    private void UpdateTimer()
    {
        _timer?.Dispose();
        if (_isRunning)
        {
            var interval = Math.Max(16, (int)(1000 / (_simulationSpeed * 60)));
            _timer = new Timer(_ => 
            {
                try
                {
                    Task.Run(() =>
                    {
                        UpdateSimulation();
                        SimulationUpdated?.Invoke(this, EventArgs.Empty);
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in simulation: {ex}");
                }
            }, null, 16, interval);
        }
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
                var foxPosition = GetRandomPosition();
                var fox = _entityFactory.CreateAnimal<Fox>(100, 100, foxPosition, i % 2 == 0);
                _worldService.AddEntity(fox);
            }

            for (int i = 0; i < 1; i++)
            {
                var rabbitPosition = GetRandomPosition();
                var rabbit = _entityFactory.CreateAnimal<Rabbit>(80, 80, rabbitPosition, i % 2 == 0);
                _worldService.AddEntity(rabbit);
            }

            for (int i = 0; i < 1; i++)
            {
                var grassPosition = GetRandomPosition();
                var grass = _entityFactory.CreatePlant<Grass>(50, 50, grassPosition);
                _worldService.AddEntity(grass);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating entities: {ex}");
        }
    }

    private Position GetRandomPosition()
    {
        return new Position(
            _random.Next(0, 800),
            _random.Next(0, 450)
        );
    }

    public void UpdateSimulation()
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            var entities = _worldService.Entities.ToList();
            foreach (var entity in entities)
            {
                if (entity is LifeForm lifeForm && !lifeForm.IsDead)
                {
                    var environment = _worldService.GetEnvironmentAt(lifeForm.Position);
                    if ((lifeForm.Environment & environment) == 0)
                    {
                        lifeForm.TakeDamage(5);
                    }

                    lifeForm.Update();

                    if (entity is Animal animal)
                    {
                        HandleAnimalBehavior(animal);
                    }
                }
            }

            CleanupDeadEntities();

            SimulationUpdated?.Invoke(this, EventArgs.Empty);
        });
    }

    private void HandleAnimalBehavior(Animal animal)
    {
        if (animal.IsDead) return;

        if (animal is Carnivore carnivore)
        {
            var prey = carnivore.FindNearestPrey();
            if (prey != null)
            {
                carnivore.MoveTowardsPrey(prey);
                if (carnivore.CanAttack(prey))
                {
                    carnivore.Attack(prey);
                }
            }
        }

        if (animal.CanReproduce())
        {
            animal.SearchForMate();
        }
    }

    private void CleanupDeadEntities()
    {
        var deadEntities = _worldService.Entities
            .OfType<LifeForm>()
            .Where(e => e.IsDead)
            .ToList();

        foreach (var entity in deadEntities)
        {
            _worldService.RemoveEntity(entity);
        }
    }
}