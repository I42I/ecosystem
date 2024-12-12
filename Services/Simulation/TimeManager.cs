using System;
using System.Collections.Generic;
using System.Threading;

namespace ecosystem.Services.Simulation;

public interface ITimeManager
{
    double CurrentTime { get; }
    void RegisterTickAction(Action action);
    void Start();
    void Pause();
    void Reset();
    void SetInterval(int milliseconds);
}

public class TimeManager : ITimeManager
{
    private readonly List<Action> _tickActions = new();
    private Timer? _timer;
    private double _currentTime;
    private bool _isRunning;
    private int _intervalMilliseconds = 1000;

    public double CurrentTime => _currentTime;

    public void RegisterTickAction(Action action)
    {
        _tickActions.Add(action);
    }

    public void Start()
    {
        if (_isRunning)
            return;

        _timer = new Timer(Tick, null, 0, _intervalMilliseconds);
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

    public void SetInterval(int milliseconds)
    {
        _intervalMilliseconds = milliseconds;
        if (_isRunning)
        {
            _timer?.Change(0, _intervalMilliseconds);
        }
    }

    private void Tick(object? state)
    {
        _currentTime += _intervalMilliseconds / 1000.0;
        foreach (var action in _tickActions)
        {
            action();
        }
    }
}