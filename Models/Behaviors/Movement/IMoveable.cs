using ecosystem.Models.Core;

namespace ecosystem.Models.Behaviors;

public interface IMoveable
{
    Position Position { get; }
    void Move(double deltaX, double deltaY);
    double GetDistanceTo(IMoveable other);
    double MovementSpeed { get; }
}
