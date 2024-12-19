using System;
using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Core;
using ecosystem.Helpers;

namespace ecosystem.Models.Behaviors.Base;

public class RestBehavior : IBehavior<Animal>
{
    public string Name => "Rest";
    public int Priority => 0;

    private int _directionChangeTicks;
    private double _currentDirectionX;
    private double _currentDirectionY;

    public bool CanExecute(Animal animal)
    {
        return true;
    }

    public void Execute(Animal animal)
    {
        Console.WriteLine($"{animal.GetType().Name} is resting");

        if (_directionChangeTicks <= 0)
        {
            double angle = RandomHelper.Instance.NextDouble() * 2 * Math.PI;
            _currentDirectionX = Math.Cos(angle);
            _currentDirectionY = Math.Sin(angle);
            _directionChangeTicks = RandomHelper.Instance.Next(60, 180);
        }

        _directionChangeTicks--;

        double variation = (RandomHelper.Instance.NextDouble() - 0.5) * 0.2;
        double dx = _currentDirectionX + variation;
        double dy = _currentDirectionY + variation;

        double length = Math.Sqrt(dx * dx + dy * dy);
        if (length > 0)
        {
            dx /= length;
            dy /= length;
        }

        animal.Move(dx, dy);
    }
}