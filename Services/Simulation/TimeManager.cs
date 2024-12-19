using System;
using System.Collections.Generic;
using Avalonia.Threading;

namespace ecosystem.Services.Simulation;

public interface ITimeManager
{
    double CurrentTime { get; }
    double DeltaTime { get; }
    void RegisterTickAction(Action action);
    void Start();
    void Pause();
    void Reset();
    void SetSimulationSpeed(double speed);
}

public class TimeManager : ITimeManager
{
    private readonly List<Action> _tickActions = new();
    private const double FIXED_TIME_STEP = 1.0 / SimulationConstants.TICK_RATE;
    private readonly GameLoop _gameLoop;
    private bool _isRunning;
    private double _currentTime;
    private double _simulationSpeed = 1.0;
    
    public double CurrentTime => _currentTime;
    public double DeltaTime => FIXED_TIME_STEP * _simulationSpeed * SimulationConstants.SIMULATION_SPEED;
    public event EventHandler? SimulationUpdated;

    public TimeManager()
    {
        _gameLoop = new GameLoop(UpdateLogic, Render);
    }

    private void UpdateLogic()
    {
        if (!_isRunning) return;
        
        foreach (var action in _tickActions)
        {
            action();
        }
        _currentTime += DeltaTime;
    }

    private void Render()
    {
        if (!_isRunning) return;
        
        Dispatcher.UIThread.Post(() => 
        {
            SimulationUpdated?.Invoke(this, EventArgs.Empty);
        });
    }

    public void RegisterTickAction(Action action)
    {
        _tickActions.Add(action);
    }

    public void Start()
    {
        _isRunning = true;
        _gameLoop.Start();
    }

    public void Pause()
    {
        _isRunning = false;
    }

    public void Reset()
    {
        Pause();
        _currentTime = 0;
        _gameLoop.Stop();
    }

    public void SetSimulationSpeed(double speed)
    {
        _simulationSpeed = speed;
    }
}