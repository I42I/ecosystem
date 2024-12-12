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

    [ObservableProperty]
    private string _status = "Ready";

    [ObservableProperty]
    private double _simulationSpeed = 1.0;

    [ObservableProperty]
    private GridWorld _world;

    public ObservableCollection<Entity> Entities => _worldService.Entities;

    public MainWindowViewModel(ISimulationEngine simulationEngine, IWorldService worldService)
    {
        Console.WriteLine("Initializing MainWindowViewModel...");
        
        _simulationEngine = simulationEngine;
        _worldService = worldService;

        _simulationEngine.SimulationUpdated += (s, e) => UpdateStatus();

        _world = worldService.Grid;
    }

    public void InitializeAndStart()
    {
        _simulationEngine.InitializeSimulation();
        _simulationEngine.Start();
        Status = "Running";
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
