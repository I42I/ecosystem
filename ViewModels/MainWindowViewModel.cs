using System;
using System.Collections.Specialized;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ecosystem.Services.Simulation;
using ecosystem.Services.World;
using ecosystem.Models.Core;
using System.Collections.ObjectModel;

namespace ecosystem.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly object _lock = new object();
    private readonly ISimulationEngine _simulationEngine;
    private readonly ITimeManager _timeManager;
    private readonly IWorldService _worldService;

    private readonly ObservableCollection<EntityViewModel> _entityViewModels;
    public ObservableCollection<EntityViewModel> EntityViewModels => _entityViewModels;

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
        _entityViewModels = new ObservableCollection<EntityViewModel>();

        if (timeManager is TimeManager tm)
        {
            tm.SimulationUpdated += (s, e) =>
            {
                lock (_lock)
                {
                    var positions = _entityViewModels.ToList();
                    foreach (var entityVM in positions)
                    {
                        entityVM.UpdateDisplaySize(WindowWidth, WindowHeight);
                    }
                }
            };
        }

        _worldService.Entities.CollectionChanged += Entities_CollectionChanged;
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
        _simulationEngine.ResetSimulation();
        Status = "Reset";
    }

    [ObservableProperty]
    private bool _isDebugMode = false;

    [RelayCommand]
    private void ToggleDebug()
    {
        IsDebugMode = !IsDebugMode;
    }

    partial void OnSimulationSpeedChanged(double value)
    {
        _timeManager.SetSimulationSpeed(value);
    }

    [ObservableProperty]
    private double _windowWidth = 800;

    [ObservableProperty]
    private double _windowHeight = 520;

    partial void OnWindowWidthChanged(double value)
    {
        foreach (var entityVM in _entityViewModels)
        {
            entityVM.UpdateDisplaySize(value, WindowHeight);
        }
    }

    partial void OnWindowHeightChanged(double value)
    {
        foreach (var entityVM in _entityViewModels)
        {
            entityVM.UpdateDisplaySize(WindowWidth, value);
        }
    }


    private void Entities_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        lock (_lock)
        {
            if (e.NewItems != null)
            {
                foreach (Entity entity in e.NewItems)
                {
                    var vm = new EntityViewModel(entity);
                    _entityViewModels.Add(vm);
                    
                    if (_timeManager is TimeManager tm)
                    {
                        tm.SimulationUpdated += (s, e) => vm.UpdateAnimation(tm.DeltaTime);
                    }
                }
            }
            if (e.OldItems != null)
            {
                foreach (Entity entity in e.OldItems)
                {
                    var vm = _entityViewModels.FirstOrDefault(vm => vm.Entity == entity);
                    if (vm != null)
                        _entityViewModels.Remove(vm);
                }
            }
        }
    }
}