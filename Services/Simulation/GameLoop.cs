using System;
using System.Threading;
using System.Threading.Tasks;

namespace ecosystem.Services.Simulation;

public class GameLoop
{
    private const int TARGET_FPS = 60;
    private const double MS_PER_UPDATE = 1000.0 / TARGET_FPS;
    
    private readonly Action _updateLogic;
    private readonly Action _render;
    private double _previousTime;
    private double _lag;
    private bool _isRunning;
    private Task? _loopTask;
    private readonly CancellationTokenSource _cancellationSource;

    public GameLoop(Action updateLogic, Action render)
    {
        _updateLogic = updateLogic;
        _render = render;
        _previousTime = GetCurrentTime();
        _cancellationSource = new CancellationTokenSource();
    }

    private double GetCurrentTime() => DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

    public void Start()
    {
        if (_isRunning) return;
        
        _isRunning = true;
        _loopTask = Task.Run(RunLoop, _cancellationSource.Token);
    }

    public void Stop()
    {
        _isRunning = false;
        _cancellationSource.Cancel();
    }

    public void WaitForStop()
    {
        try 
        {
            _loopTask?.Wait();
        }
        catch (AggregateException)
        {
            // Ignore
        }
    }

    private void RunLoop()
    {
        while (_isRunning && !_cancellationSource.Token.IsCancellationRequested)
        {
            var currentTime = GetCurrentTime();
            var elapsed = currentTime - _previousTime;
            _previousTime = currentTime;
            _lag += elapsed;

            while (_lag >= MS_PER_UPDATE)
            {
                _updateLogic();
                _lag -= MS_PER_UPDATE;
            }

            _render();

            Thread.Sleep(1);
        }
    }
}