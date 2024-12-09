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

    public virtual void Move(double deltaX, double deltaY)
    {
        Position.X += deltaX;
        Position.Y += deltaY;
    }

    public virtual double GetDistanceTo(IMoveable other)
    {
        var dx = Position.X - other.Position.X;
        var dy = Position.Y - other.Position.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}