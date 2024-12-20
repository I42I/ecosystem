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
        // Debug logs
        Console.WriteLine($"[{GetType().Name}] Move called: dx={deltaX:F3}, dy={deltaY:F3}");
        
        // Calculate frame-adjusted movement
        double frameMovement = MovementSpeed * SimulationConstants.BASE_MOVEMENT_SPEED * _timeManager.DeltaTime;
        _movementAccumulator += frameMovement;

        Console.WriteLine($"[{GetType().Name}] Movement accumulator: {_movementAccumulator:F3}");

        if (_movementAccumulator >= SimulationConstants.MOVEMENT_THRESHOLD)
        {
            double length = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            if (length > 0)
            {
                deltaX /= length;
                deltaY /= length;
            }

            double newX = Math.Clamp(Position.X + deltaX * _movementAccumulator, 0, 1);
            double newY = Math.Clamp(Position.Y + deltaY * _movementAccumulator, 0, 1);

            Console.WriteLine($"[{GetType().Name}] Position update: ({Position.X:F3}, {Position.Y:F3}) -> ({newX:F3}, {newY:F3})");

            Position = new Position(newX, newY);

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