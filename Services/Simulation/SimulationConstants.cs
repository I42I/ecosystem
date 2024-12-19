namespace ecosystem.Services.Simulation;

public static class SimulationConstants
{
    // Time settings
    public const double TICK_RATE = 60.0;
    public const double SIMULATION_SPEED = 0.2;
    
    // Update intervals
    public const double BEHAVIOR_UPDATE_INTERVAL = 0.1;
    public const double MOVEMENT_THRESHOLD = 0.016;
    public const double PLANT_GROWTH_INTERVAL = 1.0;
    public const double PLANT_REPRODUCTION_INTERVAL = 5.0;
    
    // Rates and costs
    public const double BASE_MOVEMENT_SPEED = 0.0005;
    public const double BASE_ENERGY_LOSS = 0.05;
    public const double ENVIRONMENT_DAMAGE_RATE = 0.1;
    
    // Energy costs
    public const double MOVEMENT_ENERGY_COST = 0.02;
    public const double ATTACK_ENERGY_COST = 0.1;
    public const double REPRODUCTION_ENERGY_COST = 0.5;
}