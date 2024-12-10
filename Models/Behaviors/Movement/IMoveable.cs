using ecosystem.Models.Core;

namespace ecosystem.Models.Behaviors.Movement;

public interface IMoveable
{
    Position Position { get; }
    void Move(double deltaX, double deltaY);
    double MovementSpeed { get; }
}
