using System;
using ecosystem.Models.Core;

namespace ecosystem.Helpers;

public static class RandomHelper
{
    private static readonly object lockObject = new object();
    private static Random? instance;

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

    private static int seed;

    public static void Initialize(int seedValue)
    {
        lock (lockObject)
        {
            seed = seedValue;
            instance = new Random(seed);
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
            return (
                centerX + distance * Math.Cos(angle),
                centerY + distance * Math.Sin(angle)
            );
        }
    }

    public static Position GetRandomPosition(double maxX, double maxY)
    {
        var x = Instance.NextDouble() * maxX;
        var y = Instance.NextDouble() * maxY;
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
