using System;
using System.Collections.Generic;
using System.Threading;

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
    private Timer? _timer;
    private double _currentTime;
    private bool _isRunning;
    private double _simulationSpeed = 1.0;
    private int _baseIntervalMilliseconds = 1000;
    public double CurrentTime => _currentTime;
    private readonly double _fixedDeltaTime = 1.0 / 60.0;
    public double DeltaTime => _fixedDeltaTime * _simulationSpeed;

    public void RegisterTickAction(Action action)
    {
        _tickActions.Add(action);
    }

    public void Start()
    {
        if (_isRunning)
            return;

        _timer = new Timer(Tick, null, 0, GetInterval());
        _isRunning = true;
    }

    public void Pause()
    {
        if (!_isRunning)
            return;

        _timer?.Change(Timeout.Infinite, Timeout.Infinite);
        _isRunning = false;
    }

    public void Reset()
    {
        Pause();
        _currentTime = 0;
    }

    public void SetSimulationSpeed(double speed)
    {
        _simulationSpeed = speed;
        if (_isRunning)
        {
            _timer?.Change(0, GetInterval());
        }
    }

    private int GetInterval()
    {
        return (int)(_baseIntervalMilliseconds / _simulationSpeed);
    }

    private void Tick(object? state)
    {
        _currentTime += _baseIntervalMilliseconds / 1000.0;
        foreach (var action in _tickActions)
        {
            action();
        }
    }
}