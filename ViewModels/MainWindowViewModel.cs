using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ecosystem.Services.Simulation;
using ecosystem.Services.World;
using ecosystem.Models.Core;

namespace ecosystem.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ISimulationEngine _simulationEngine;
    private readonly ITimeManager _timeManager;
    private readonly IWorldService _worldService;

    [ObservableProperty]
    private string _status = "Ready";

    [ObservableProperty]
    private double _simulationSpeed = 1.0;

    public System.Collections.ObjectModel.ObservableCollection<Entity> Entities => _worldService.Entities;

    [ObservableProperty]
    private bool _debug = true;

    public MainWindowViewModel(ISimulationEngine simulationEngine, ITimeManager timeManager, IWorldService worldService)
    {
        _simulationEngine = simulationEngine;
        _timeManager = timeManager;
        _worldService = worldService;
    }

    public void InitializeAndStart()
    {
        Console.WriteLine($"Initial entities count: {_worldService.Entities.Count}");
        _simulationEngine.InitializeSimulation();
        Console.WriteLine($"Entities after init: {_worldService.Entities.Count}");
        _timeManager.Start();
        Status = "Running";
    }

    [RelayCommand]
    private void StartSimulation()
    {
        _timeManager.Start();
        Status = "Running";
    }

    [RelayCommand]
    private void PauseSimulation()
    {
        _timeManager.Pause();
        Status = "Paused";
    }

    [RelayCommand]
    private void ResetSimulation()
    {
        _timeManager.Reset();
        _simulationEngine.InitializeSimulation();
        Status = "Reset";
    }

    partial void OnSimulationSpeedChanged(double value)
    {
        _timeManager.SetSimulationSpeed(value);
    }

    [ObservableProperty]
    private double _windowWidth = 800;

    [ObservableProperty]
    private double _windowHeight = 450;

    partial void OnWindowWidthChanged(double value)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() => 
        {
            _worldService.UpdateDisplaySize(value, WindowHeight);
        });
    }

    partial void OnWindowHeightChanged(double value)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() => 
        {
            _worldService.UpdateDisplaySize(WindowWidth, value);
        });
    }
}