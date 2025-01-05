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
    public const double MALE_REPRODUCTION_COOLDOWN = 10.0;
    public const double GESTATION_PERIOD = 0.5;
    public const double FEMALE_REPRODUCTION_COOLDOWN = 15.0;

    // Energy thresholds
    public const double MAX_ENERGY_PERCENT = 100.0;
    public const double HEALING_ENERGY_THRESHOLD = 90.0;
    public const double HEALING_CONVERSION_RATE = 1;
    
    // Health thresholds
    public const double MAX_HEALTH_PERCENT = 100.0;

    // Organic waste constants
    public const double WASTE_CONTACT_RADIUS = 0.005;
    public const double WASTE_DECAY_RATE = 0.001;
    public const double WASTE_BASE_ENERGY = 100;
    public const double WASTE_MIN_ENERGY = 10;

    // Root system constants
    public const double ROOT_GROWTH_RATE = 0.0003;
    public const double MAX_ROOT_RADIUS = 0.2;
    public const double MIN_ROOT_RADIUS = 0.05;
    public const double WASTE_ABSORPTION_THRESHOLD = 0.001;

    // Plant reproduction constants
    public const double PLANT_SEED_SPREAD_BASE_CHANCE = 0.05;
    public const int PLANT_MIN_ENERGY_FOR_REPRODUCTION = 50;
    public const int PLANT_MIN_HEALTH_FOR_REPRODUCTION = 30;
    public const int PLANT_REPRODUCTION_ENERGY_COST = 30;
    public const double PLANT_REPRODUCTION_CHECK_INTERVAL = 0.5;

    // Digestion constants
    public const double DIGESTION_TIME = 0.1;
    public const double DIGESTION_EFFICIENCY = 0.7;
    public const int MIN_WASTE_PER_DIGESTION = 1;
    public const double WASTE_DROP_RADIUS = 0.01;
}