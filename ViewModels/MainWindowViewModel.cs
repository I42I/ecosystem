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

    public MainWindowViewModel(ISimulationEngine simulationEngine, ITimeManager timeManager, IWorldService worldService)
    {
        _simulationEngine = simulationEngine;
        _timeManager = timeManager;
        _worldService = worldService;

        _simulationEngine.InitializeSimulation();
        Status = "Initialized";
    }

    public void InitializeAndStart()
    {
        _simulationEngine.InitializeSimulation();
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
}