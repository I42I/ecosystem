using System;
using ecosystem.Models.Core;
using ecosystem.Models.Entities.Environment;
using ecosystem.Services.Simulation;

namespace ecosystem.Models.Behaviors.Movement;

public abstract class MoveableEntity : LifeForm, IMoveable
{
    private double _energyCostAccumulator;  
    protected double BasalMetabolicRate { get; }
    protected abstract double SpeciesEnergyCostModifier { get; }

    protected MoveableEntity(
        Position position,
        int healthPoints,
        int energy,
        EnvironmentType environment,
        double basalMetabolicRate,
        ITimeManager timeManager)
        : base(position, healthPoints, energy, environment, timeManager)
    {
        BasalMetabolicRate = basalMetabolicRate;
    }

    public virtual double MovementSpeed { get; set; }

    protected double _currentDirectionX = 0;
    protected double _currentDirectionY = 0;
    protected int _directionChangeTicks = 0;

    public virtual void Move(double deltaX, double deltaY)
    {
        double frameMovement = MovementSpeed * SimulationConstants.BASE_MOVEMENT_SPEED * _timeManager.DeltaTime;
        
        double length = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        if (length > 0)
        {
            deltaX /= length;
            deltaY /= length;
        }

        double newX = Math.Clamp(Position.X + deltaX * frameMovement, 0, 1);
        double newY = Math.Clamp(Position.Y + deltaY * frameMovement, 0, 1);

        Position = new Position(newX, newY);

        _energyCostAccumulator += CalculateMovementEnergyCost(deltaX * frameMovement, deltaY * frameMovement);

        if (_energyCostAccumulator >= 1)
        {
            ConsumeEnergy((int)Math.Floor(_energyCostAccumulator));
            _energyCostAccumulator -= Math.Floor(_energyCostAccumulator);
        }
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