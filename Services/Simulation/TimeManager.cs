using System;
using System.Collections.Generic;
using Avalonia.Threading;
using System.Threading;

namespace ecosystem.Services.Simulation;

public interface ITimeManager
{
    double CurrentTime { get; }
    double DeltaTime { get; }
    void RegisterTickAction(Action action);
    void Start();
    void Pause();
    void WaitForStop();
    void Reset();
    void SetSimulationSpeed(double speed);
}

public class TimeManager : ITimeManager
{
    private readonly List<Action> _tickActions = new();
    private const double FIXED_TIME_STEP = 1.0 / SimulationConstants.TICK_RATE;
    private GameLoop _gameLoop;
    private bool _isRunning;
    private double _currentTime;
    private double _simulationSpeed = 1.0;
    private bool _isResetting;
    
    public double CurrentTime => _currentTime;
    public double DeltaTime => FIXED_TIME_STEP * _simulationSpeed * SimulationConstants.SIMULATION_SPEED;
    public event EventHandler? SimulationUpdated;

    private double _displayTime;
    private const double DISPLAY_TIME_MULTIPLIER = 10.0;

    public TimeManager()
    {
        _gameLoop = new GameLoop(UpdateLogic, Render);
    }

    private void UpdateLogic()
    {
        if (!_isRunning || _isResetting) return;
        
        foreach (var action in _tickActions)
        {
            action();
        }
        _currentTime += DeltaTime;
        _displayTime += DeltaTime * DISPLAY_TIME_MULTIPLIER;
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

    public void WaitForStop()
    {
        _isResetting = true;
        while (_isRunning)
        {
            Thread.Sleep(10);
        }
        _isResetting = false;
    }

    public double DisplayTime => _displayTime;

    public void Reset()
    {
        _isResetting = true;
        Pause();
        if (_gameLoop != null)
        {
            _gameLoop.Stop();
            _gameLoop.WaitForStop();
        }
        _currentTime = 0;
        _displayTime = 0;
        _simulationSpeed = 1.0;
        _gameLoop = new GameLoop(UpdateLogic, Render);
        _isResetting = false;
        Start();
    }

    public void SetSimulationSpeed(double speed)
    {
        _simulationSpeed = speed;
    }
}