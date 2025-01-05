using System;
using ecosystem.Models.Core;
using ecosystem.Models.Entities.Environment;
using ecosystem.Services.Simulation;
using ecosystem.Services.World;
using ecosystem.Models.Radius;

namespace ecosystem.Models.Behaviors.Movement;

public abstract class MoveableEntity : LifeForm, IMoveable
{
    private double _energyCostAccumulator;
    protected double BasalMetabolicRate { get; }
    protected abstract double SpeciesEnergyCostModifier { get; }
    protected readonly IWorldService _worldService;

    protected MoveableEntity(
        Position position,
        int healthPoints,
        int energy,
        EnvironmentType environment,
        double basalMetabolicRate,
        ITimeManager timeManager,
        IWorldService worldService)
        : base(position, healthPoints, energy, environment, timeManager)
    {
        BasalMetabolicRate = basalMetabolicRate;
        _previousPosition = position;
        _worldService = worldService;
    }

    public virtual double MovementSpeed { get; set; }

    protected double _currentDirectionX = 0;
    public double CurrentDirectionX => _currentDirectionX;
    protected double _currentDirectionY = 0;
    protected Position? _previousPosition;

    private DateTime _lastMoveTime = DateTime.Now;
    private const double MOVEMENT_TIMEOUT = 0.1;
    private bool _wasMoving = false;
    public bool IsMoving
    {
        get
        {
            bool currentlyMoving = (DateTime.Now - _lastMoveTime).TotalSeconds < MOVEMENT_TIMEOUT;
            
            if (currentlyMoving != _wasMoving)
            {
                _wasMoving = currentlyMoving;
            }
            
            return currentlyMoving;
        }
    }

    private const double AVOIDANCE_START_THRESHOLD = 1.2;
    private const double HARD_AVOIDANCE_THRESHOLD = 1.5;
    private const double DANGER_CHECK_DISTANCE = 0.05;
    private const int DIRECTION_SAMPLES = 12;
    
    public virtual void Move(double deltaX, double deltaY)
    {
        if (this is IEnvironmentSensitive sensitive)
        {
            var currentEnv = _worldService.GetEnvironmentAt(Position);
            var futureX = Position.X + deltaX * DANGER_CHECK_DISTANCE;
            var futureY = Position.Y + deltaY * DANGER_CHECK_DISTANCE;
            var futurePos = new Position(futureX, futureY);
            var futureEnv = _worldService.GetEnvironmentAt(futurePos);

            var currentPreference = sensitive.GetBestEnvironmentPreference(currentEnv);
            var futurePreference = sensitive.GetBestEnvironmentPreference(futureEnv);

            double avoidanceFactor = CalculateAvoidanceFactor(currentPreference.EnergyLossModifier, futurePreference.EnergyLossModifier);

            if (avoidanceFactor > 0)
            {
                var safeDirection = FindSafeDirection(deltaX, deltaY, avoidanceFactor);
                if (safeDirection.HasValue)
                {
                    deltaX = Lerp(deltaX, safeDirection.Value.x, avoidanceFactor);
                    deltaY = Lerp(deltaY, safeDirection.Value.y, avoidanceFactor);
                }
            }
        }

        double frameMovement = MovementSpeed * SimulationConstants.BASE_MOVEMENT_SPEED * _timeManager.DeltaTime;
        Position = new Position(
            Math.Clamp(Position.X + deltaX * frameMovement, 0, 1),
            Math.Clamp(Position.Y + deltaY * frameMovement, 0, 1)
        );
        
        _currentDirectionX = Math.Sign(deltaX);
        _currentDirectionY = Math.Sign(deltaY);
        _lastMoveTime = DateTime.Now;

        _energyCostAccumulator += CalculateMovementEnergyCost(deltaX * frameMovement, deltaY * frameMovement);
        if (_energyCostAccumulator >= 1)
        {
            ConsumeEnergy((int)Math.Floor(_energyCostAccumulator));
            _energyCostAccumulator -= Math.Floor(_energyCostAccumulator);
        }
    }

    private double CalculateAvoidanceFactor(double currentDanger, double futureDanger)
    {
        double maxDanger = Math.Max(currentDanger, futureDanger);
        if (maxDanger <= AVOIDANCE_START_THRESHOLD)
            return 0;
        if (maxDanger >= HARD_AVOIDANCE_THRESHOLD)
            return 1;
            
        return (maxDanger - AVOIDANCE_START_THRESHOLD) / 
               (HARD_AVOIDANCE_THRESHOLD - AVOIDANCE_START_THRESHOLD);
    }

    private (double x, double y)? FindSafeDirection(double originalDx, double originalDy, double avoidanceFactor)
    {
        if (!(this is IEnvironmentSensitive sensitive)) return null;

        double bestScore = double.MinValue;
        (double x, double y)? bestDirection = null;
        
        double targetAngle = Math.Atan2(originalDy, originalDx);

        int samples = (int)(DIRECTION_SAMPLES * (1 + avoidanceFactor));
        
        for (int i = 0; i < samples; i++)
        {
            double angleRange = Math.PI * (0.5 + 0.5 * avoidanceFactor);
            double angle = targetAngle + (angleRange * (i - samples/2)) / (samples/2);
            
            double dx = Math.Cos(angle);
            double dy = Math.Sin(angle);
            
            var futureX = Position.X + dx * DANGER_CHECK_DISTANCE;
            var futureY = Position.Y + dy * DANGER_CHECK_DISTANCE;
            var futurePos = new Position(futureX, futureY);
            
            var envType = _worldService.GetEnvironmentAt(futurePos);
            var preference = sensitive.GetBestEnvironmentPreference(envType);
            
            double envScore = 1.0 / preference.EnergyLossModifier;
            double angleScore = Math.Cos(angle - targetAngle);
            double distanceFromOriginal = 1.0 - (Math.Abs(angle - targetAngle) / Math.PI);
            
            double score = Lerp(
                angleScore,
                envScore * distanceFromOriginal,
                avoidanceFactor
            );
            
            if (score > bestScore)
            {
                bestScore = score;
                bestDirection = (dx, dy);
            }
        }

        return bestDirection;
    }

    private double Lerp(double a, double b, double t)
    {
        return a * (1 - t) + b * t;
    }

    protected virtual double CalculateMovementEnergyCost(double deltaX, double deltaY)
    {
        double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        double environmentModifier = GetEnvironmentMovementModifier();
        
        return distance * BasalMetabolicRate * environmentModifier * 
               SpeciesEnergyCostModifier * SimulationConstants.MOVEMENT_ENERGY_COST;
    }

    protected virtual double GetEnvironmentMovementModifier()
    {
        var preference = (this as IEnvironmentSensitive)?.GetBestEnvironmentPreference(Environment);
        return preference?.MovementModifier ?? 1.0;
    }

    public void AddEnergy(int amount)
    {
        Energy += amount;
    }
    
    public void RemoveEnergy(int amount)
    {
        Energy -= amount;
    }
}