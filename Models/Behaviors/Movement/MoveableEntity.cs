using System;
using ecosystem.Models.Core;
using ecosystem.Models.Entities.Environment;
using ecosystem.Services.Simulation;

namespace ecosystem.Models.Behaviors.Movement;

public abstract class MoveableEntity : LifeForm, IMoveable
{
    private double _movementAccumulator;
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

    public virtual double MovementSpeed { get; protected set; }

    protected double _currentDirectionX = 0;
    protected double _currentDirectionY = 0;
    protected int _directionChangeTicks = 0;

    public virtual void Move(double deltaX, double deltaY)
    {
        // Calculate frame-adjusted movement
        double frameMovement = MovementSpeed * SimulationConstants.BASE_MOVEMENT_SPEED * _timeManager.DeltaTime;
        _movementAccumulator += frameMovement;

        if (_movementAccumulator >= SimulationConstants.MOVEMENT_THRESHOLD)
        {
            double length = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            if (length > 0)
            {
                deltaX /= length;
                deltaY /= length;
            }

            Position = new Position(
                Math.Clamp(Position.X + deltaX * _movementAccumulator, 0, 1),
                Math.Clamp(Position.Y + deltaY * _movementAccumulator, 0, 1)
            );

            // Calculate and accumulate movement energy cost
            double energyCost = CalculateMovementEnergyCost(
                deltaX * _movementAccumulator, 
                deltaY * _movementAccumulator
            );
            
            ConsumeEnergy(energyCost);
            _movementAccumulator = 0;
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