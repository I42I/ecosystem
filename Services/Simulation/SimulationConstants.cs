namespace ecosystem.Services.Simulation;

public static class SimulationConstants
{
    // Time settings
    public const double TICK_RATE = 60.0;
    public const double SIMULATION_SPEED = 0.1;
    
    // Movement thresholds
    public const double MOVEMENT_THRESHOLD = 0.0001;
    public const double BASE_MOVEMENT_SPEED = 0.5;
    
    // Update intervals
    public const double BEHAVIOR_UPDATE_INTERVAL = 0.01;
    public const double PLANT_GROWTH_INTERVAL = 1.0;
    public const double PLANT_REPRODUCTION_INTERVAL = 5.0;
    
    // Energy costs
    public const double BASE_ENERGY_LOSS = 0.05;
    public const double MOVEMENT_ENERGY_COST = 0.02;
    public const double ENVIRONMENT_DAMAGE_RATE = 0.1;
    public const double ATTACK_ENERGY_COST = 0.1;
    public const double REPRODUCTION_ENERGY_COST = 0.5;

    // Health costs
    public const double BASE_HEALTH_LOSS = 0.01;
    public const double HEALTH_LOSS_WHEN_STARVING = 0.02;
    public const double MIN_HEALTH_FOR_CONVERSION = 10;
    public const double ENERGY_HEALTH_CONVERSION_RATE = 10;

    // Reproduction timings
    public const double MALE_REPRODUCTION_COOLDOWN = 30.0;
    public const double GESTATION_PERIOD = 60.0;
    public const double FEMALE_REPRODUCTION_COOLDOWN = 40.0;
}