using System;
using ecosystem.Models.Core;

namespace ecosystem.Helpers;

public static class RandomHelper
{
    private static readonly object lockObject = new object();
    private static Random? instance;
    private const double BORDER_MARGIN = 0.05;
    private static int seed = 42;

    public static Random Instance
    {
        get
        {
            lock (lockObject)
            {
                return instance ??= new Random(seed);
            }
        }
    }

    public static void Initialize(int seedValue)
    {
        lock (lockObject)
        {
            seed = seedValue;
            instance = new Random(seed);
            Console.WriteLine($"Initialized RandomHelper with seed: {seed}");
        }
    }

    public static double NextDouble()
    {
        lock (lockObject)
        {
            return Instance.NextDouble();
        }
    }

    public static (double X, double Y) GetRandomPositionInRadius(double centerX, double centerY, double radius)
    {
        lock (lockObject)
        {
            double angle = Instance.NextDouble() * 2 * Math.PI;
            double distance = Instance.NextDouble() * radius;
            double newX = Math.Clamp(centerX + distance * Math.Cos(angle), 0, 1);
            double newY = Math.Clamp(centerY + distance * Math.Sin(angle), 0, 1);
            
            return (newX, newY);
        }
    }

    public static Position GetRandomPosition()
    {
        var x = BORDER_MARGIN + Instance.NextDouble() * (1 - 2 * BORDER_MARGIN);
        var y = BORDER_MARGIN + Instance.NextDouble() * (1 - 2 * BORDER_MARGIN);
        return new Position { X = x, Y = y };
    }

    public static double NextGaussian(double mean, double standardDeviation)
    {
        lock (lockObject)
        {
            double u1 = 1.0 - Instance.NextDouble();
            double u2 = 1.0 - Instance.NextDouble();
            double randomStandardNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + standardDeviation * randomStandardNormal;
        }
    }

    public static (double X, double Y) GetRandomDirection()
    {
        lock (lockObject)
        {
            double angle = Instance.NextDouble() * 2 * Math.PI;
            return (Math.Cos(angle), Math.Sin(angle));
        }
    }
}
