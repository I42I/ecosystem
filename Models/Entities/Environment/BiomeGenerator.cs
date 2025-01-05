using System;
using System.Collections.Generic;

namespace ecosystem.Models.Entities.Environment;

public static class BiomeGenerator
{
    public static List<(float x, float y, float radius)> GenerateWaterCenters(
        int width, int height, int seed, int centerCount = 2)
    {
        var rnd = new Random(seed);
        var centers = new List<(float x, float y, float radius)>();

        for (int i = 0; i < centerCount; i++)
        {
            float x = rnd.Next(width);
            float y = rnd.Next(height);
            float radius = (float)(rnd.NextDouble() * Math.Min(width, height) / 3) + 
                         Math.Min(width, height) / 6;
            centers.Add((x, y, radius));
        }

        return centers;
    }
}