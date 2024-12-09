using System;
using ecosystem.Models.Behaviors;

namespace ecosystem.Models.Core;

public abstract class MoveableEntity(Position position) : Entity(position), IMoveable
{
    public virtual double MovementSpeed { get; protected set; }

    public virtual void Move(double deltaX, double deltaY)
    {
        Position = new Position(Position.X + deltaX, Position.Y + deltaY);
    }

    public virtual double GetDistanceTo(IMoveable other)
    {
        var dx = Position.X - other.Position.X;
        var dy = Position.Y - other.Position.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}
