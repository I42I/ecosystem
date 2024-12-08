using System;

namespace ecosystem.Services.Simulation;

public interface ISimulationEngine
{
    event EventHandler? SimulationUpdated;
    double SimulationSpeed { get; set; }
    void Start();
    void Pause();
    void Reset();
    void InitializeSimulation();
    void UpdateSimulation();
}