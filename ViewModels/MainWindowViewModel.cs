using System;
using System.Collections.Specialized;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ecosystem.Services.Simulation;
using ecosystem.Services.World;
using ecosystem.Models.Core;
using System.Collections.ObjectModel;
using ecosystem.Helpers;
using Avalonia.Media;
using ecosystem.Models.Entities.Environment;

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

    [ObservableProperty]
    private string _simulationTime = "00:00:00";
    
    [ObservableProperty] 
    private int _simulationSeed = RandomHelper.Seed;

    private ObservableCollection<GridCellViewModel> _gridCells;
    public ObservableCollection<GridCellViewModel> GridCells => _gridCells;

    public MainWindowViewModel(ISimulationEngine simulationEngine, ITimeManager timeManager, IWorldService worldService)
    {
        Console.WriteLine("Initializing MainWindowViewModel");
        _simulationEngine = simulationEngine;
        _timeManager = timeManager;
        _worldService = worldService;
        _entityViewModels = new ObservableCollection<EntityViewModel>();
        _gridCells = new ObservableCollection<GridCellViewModel>();
        
        Console.WriteLine("Updating grid cells");
        UpdateGridCells();
        Console.WriteLine($"Grid cells count: {_gridCells.Count}");

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

                    var time = TimeSpan.FromSeconds(tm.DisplayTime);
                    SimulationTime = $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
                }
            };
        }

        _worldService.Entities.CollectionChanged += Entities_CollectionChanged;
    }

    private void UpdateGridCells()
    {
        _gridCells.Clear();
        
        var cellWidth = WindowWidth / _worldService.Grid.Width;
        var cellHeight = WindowHeight / _worldService.Grid.Height;

        for (int x = 0; x < _worldService.Grid.Width; x++)
        {
            for (int y = 0; y < _worldService.Grid.Height; y++)
            {
                var envType = _worldService.Grid.GetEnvironmentAt(x, y);
                var color = envType == EnvironmentType.Water ? 
                    new SolidColorBrush(Colors.DeepSkyBlue) : 
                    new SolidColorBrush(Colors.ForestGreen);

                _gridCells.Add(new GridCellViewModel(
                    x * cellWidth,
                    y * cellHeight,
                    cellWidth + 1,
                    cellHeight + 1,
                    color
                ));
            }
        }
    }

    public void InitializeAndStart()
    {
        Console.WriteLine($"Initial entities count: {_worldService.Entities.Count}");
        _simulationEngine.InitializeSimulation();
        Console.WriteLine($"Entities after init: {_worldService.Entities.Count}");
        _timeManager.Start();
        Status = "Running";
    }

    [ObservableProperty] 
    private bool _isRunning = true;

    [RelayCommand]
    private void ToggleSimulation()
    {
        if (IsRunning)
        {
            _timeManager.Pause();
            Status = "Paused";
        }
        else
        {
            _timeManager.Start();
            Status = "Running"; 
        }
        IsRunning = !IsRunning;
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
    private double _windowHeight = 600;

    partial void OnWindowWidthChanged(double value)
    {
        UpdateGridCells();
        foreach (var entityVM in _entityViewModels)
        {
            entityVM.UpdateDisplaySize(value, WindowHeight);
        }
    }

    partial void OnWindowHeightChanged(double value)
    {
        UpdateGridCells();
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