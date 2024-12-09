using System;
using ecosystem.Models.Behaviors;
using ecosystem.Models.Entities.Environment;

namespace ecosystem.Models.Core;

public abstract class MoveableEntity : LifeForm, IMoveable
{
    protected MoveableEntity(
        int healthPoints,
        int energy,
        Position position,
        double basalMetabolicRate,
        EnvironmentType environment)
        : base(healthPoints, energy, position, basalMetabolicRate, environment)
    {
    }

    public virtual double MovementSpeed { get; protected set; }

    protected abstract int CalculateMovementEnergyCost(double deltaX, double deltaY);

    protected double _currentDirectionX = 0;
    protected double _currentDirectionY = 0;
    protected int _directionChangeTicks = 0;

    public virtual void Move(double deltaX, double deltaY)
    {
        Console.WriteLine($"Moving from ({Position.X}, {Position.Y}) by ({deltaX}, {deltaY})");

        double frameAdjustedSpeed = MovementSpeed * (1.0/10.0);
        
        double scaledDeltaX = deltaX * frameAdjustedSpeed;
        double scaledDeltaY = deltaY * frameAdjustedSpeed;

        Position.X += scaledDeltaX;
        Position.Y += scaledDeltaY;

        _accumulatedEnergyCost += CalculateMovementEnergyCost(scaledDeltaX, scaledDeltaY);
    
        if (_accumulatedEnergyCost >= 1)
        {
            int energyToConsume = (int)Math.Floor(_accumulatedEnergyCost);
            ConsumeEnergy(energyToConsume);
            _accumulatedEnergyCost -= energyToConsume;
        }
        Console.WriteLine($"New position: ({Position.X}, {Position.Y})");
    }

    private double _accumulatedEnergyCost = 0;

    public virtual double GetDistanceTo(IMoveable other)
    {
        var dx = Position.X - other.Position.X;
        var dy = Position.Y - other.Position.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}