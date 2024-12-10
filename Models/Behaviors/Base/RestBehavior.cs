using System;
using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Core;
using ecosystem.Helpers;

namespace ecosystem.Models.Behaviors.Base;

public class RestBehavior : IBehavior<Animal>
{
    public int Priority => 0;
    
    public bool CanExecute(Animal animal)
    {
        return animal is Animal;
    }
    
    public void Execute(Animal animal)
    {
        if (animal is Animal animal)
        {
            Console.WriteLine($"{GetType().Name} is resting");
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
            
            Move(dx, dy);

        }
    }
}