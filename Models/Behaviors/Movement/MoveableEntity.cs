using System;
using ecosystem.Models.Core;
using ecosystem.Models.Entities.Environment;
using ecosystem.Services.Simulation;

namespace ecosystem.Models.Behaviors.Movement;

public abstract class MoveableEntity : LifeForm, IMoveable
{
    protected readonly ITimeManager _timeManager;
    protected double BasalMetabolicRate { get; }
    protected abstract double SpeciesEnergyCostModifier { get; }

    protected MoveableEntity(
        Position position,
        int healthPoints,
        int energy,
        EnvironmentType environment,
        double basalMetabolicRate,
        ITimeManager timeManager)
        : base(position, healthPoints, energy, environment)
    {
        _timeManager = timeManager;
        BasalMetabolicRate = basalMetabolicRate;
    }
    public virtual double MovementSpeed { get; protected set; }

    protected virtual int CalculateMovementEnergyCost(double deltaX, double deltaY)
    {
        double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        double environmentModifier = GetEnvironmentMovementModifier();
        
        return (int)(distance * BasalMetabolicRate * environmentModifier * SpeciesEnergyCostModifier);
    }

    protected double _currentDirectionX = 0;
    protected double _currentDirectionY = 0;
    protected int _directionChangeTicks = 0;

    private double _accumulatedEnergyCost = 0;

    public virtual void Move(double deltaX, double deltaY)
    {
        double frameAdjustedSpeed = MovementSpeed * _timeManager.DeltaTime;
        
        double length = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        if (length > 0)
        {
            deltaX /= length;
            deltaY /= length;
        }

        double newX = Position.X + deltaX * frameAdjustedSpeed;
        double newY = Position.Y + deltaY * frameAdjustedSpeed;

        Position = new Position(
            Math.Clamp(newX, 0, 1),
            Math.Clamp(newY, 0, 1)
        );

        _accumulatedEnergyCost += CalculateMovementEnergyCost(deltaX * frameAdjustedSpeed, deltaY * frameAdjustedSpeed);
        if (_accumulatedEnergyCost >= 1)
        {
            int energyToConsume = (int)Math.Floor(_accumulatedEnergyCost);
            ConsumeEnergy(energyToConsume);
            _accumulatedEnergyCost -= energyToConsume;
        }
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