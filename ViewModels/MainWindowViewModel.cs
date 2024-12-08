using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ecosystem.Services.Factory;
using ecosystem.Services.Simulation;
using ecosystem.Services.World;
using ecosystem.Models.Entities.Environment;
using System.Collections.ObjectModel;
using ecosystem.Models.Core;
using ecosystem.Models.Entities.Animals.Carnivores;
using ecosystem.Models.Entities.Animals.Herbivores;
using ecosystem.Models.Entities.Plants;
using System;
using System.Threading.Tasks;

namespace ecosystem.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ISimulationEngine _simulationEngine;
    private readonly IWorldService _worldService;
    private readonly IEntityFactory _entityFactory;

    [ObservableProperty]
    private string _status = "Ready";

    [ObservableProperty]
    private double _simulationSpeed = 1.0;

    [ObservableProperty]
    private GridWorld _world;

    public ObservableCollection<Entity> Entities => _worldService.Entities;

    public MainWindowViewModel(
        ISimulationEngine simulationEngine,
        IWorldService worldService,
        IEntityFactory entityFactory)
    {
        Console.WriteLine("Initializing MainWindowViewModel...");
        
        _simulationEngine = simulationEngine;
        _worldService = worldService;
        _entityFactory = entityFactory;

        _simulationEngine.SimulationUpdated += (s, e) => 
        {
            Dispatcher.UIThread.Post(UpdateStatus, DispatcherPriority.Background);
        };

        _world = worldService.Grid;
    }

    public void InitializeAndStart()
    {
        try
        {
            Console.WriteLine("Initializing simulation...");
            InitializeSimulation();
            
            Console.WriteLine("Starting simulation...");
            Dispatcher.UIThread.Post(() =>
            {
                StartSimulation();
                Status = $"Running - Entities: {_worldService.Entities.Count}";
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during initialization: {ex}");
        }
    }

    private void InitializeSimulation()
    {
        CreateInitialEntities();
    }

    private void CreateInitialEntities()
    {
        try
        {
            Console.WriteLine("Creating initial entities...");
            
            for (int i = 0; i < 5; i++)
            {
                var fox = _entityFactory.CreateAnimal<Fox>(100, 100, 
                    (100 + i * 50, 100 + i * 30), true);
                _worldService.AddEntity(fox);
            }

            for (int i = 0; i < 10; i++)
            {
                var rabbit = _entityFactory.CreateAnimal<Rabbit>(80, 80, 
                    (200 + i * 40, 200 + i * 20), i % 2 == 0);
                _worldService.AddEntity(rabbit);
            }

            for (int i = 0; i < 20; i++)
            {
                var grass = _entityFactory.CreatePlant<Grass>(50, 50, 
                    (300 + i * 20, 300 + i * 10));
                _worldService.AddEntity(grass);
            }

            Console.WriteLine($"Created {_worldService.Entities.Count} entities");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating entities: {ex}");
        }
    }

    [RelayCommand]
    private void StartSimulation()
    {
        _simulationEngine.Start();
        Status = "Running";
    }

    [RelayCommand]
    private void PauseSimulation()
    {
        _simulationEngine.Pause();
        Status = "Paused";
    }

    [RelayCommand]
    private void ResetSimulation()
    {
        _simulationEngine.Reset();
        Status = "Reset";
        InitializeSimulation();
    }

    private void UpdateStatus()
    {
        Status = $"Running - Entities: {_worldService.Entities.Count}";
        Console.WriteLine($"Status updated: {Status}");
    }

    partial void OnSimulationSpeedChanged(double value)
    {
        _simulationEngine.SimulationSpeed = value;
    }
}
