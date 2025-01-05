using System;
using ecosystem.Models.Core;
using ecosystem.Helpers;
using System.Collections.Generic;

namespace ecosystem.Models.Entities.Environment;

public class GridWorld 
{
    private EnvironmentType[,] _grid;
    private const int GRID_WIDTH = 20;
    private const int GRID_HEIGHT = 13;

    public int Width => GRID_WIDTH;
    public int Height => GRID_HEIGHT;

    public GridWorld(int displayWidth, int displayHeight)
    {
        _grid = new EnvironmentType[GRID_WIDTH, GRID_HEIGHT];
        GenerateNaturalBiomes();
    }

    private void GenerateNaturalBiomes()
    {
        var waterSources = new List<(int x, int y, float radius, float noiseScale)>();
        
        var sourceCount = RandomHelper.Instance.Next(2, 4);
        for (int i = 0; i < sourceCount; i++)
        {
            waterSources.Add((
                RandomHelper.Instance.Next(GRID_WIDTH),
                RandomHelper.Instance.Next(GRID_HEIGHT),
                RandomHelper.Instance.Next(3, 7),
                (float)(RandomHelper.Instance.NextDouble() * 0.5 + 0.5)
            ));
        }

        for (int x = 0; x < GRID_WIDTH; x++)
        {
            for (int y = 0; y < GRID_HEIGHT; y++)
            {
                bool isWater = false;
                foreach (var source in waterSources)
                {
                    float dx = x - source.x;
                    float dy = y - source.y;
                    float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                    float noiseValue = (float)(
                        RandomHelper.NextGaussian(0, source.noiseScale) + 
                        Math.Sin(x * 0.5) * 0.5 + 
                        Math.Cos(y * 0.5) * 0.5
                    );
                    
                    if (distance < source.radius + noiseValue)
                    {
                        isWater = true;
                        break;
                    }
                }
                _grid[x, y] = isWater ? EnvironmentType.Water : EnvironmentType.Ground;
            }
        }

        EnsureMaxWaterCoverage(0.4f);

        ApplySmoothingPass();
        ApplySmoothingPass();
    }

    private void EnsureMaxWaterCoverage(float maxRatio)
    {
        var waterCount = 0;
        var totalCells = GRID_WIDTH * GRID_HEIGHT;
        for (int x = 0; x < GRID_WIDTH; x++)
        {
            for (int y = 0; y < GRID_HEIGHT; y++)
            {
                if (_grid[x,y] == EnvironmentType.Water)
                    waterCount++;
            }
        }

        var ratio = waterCount / (float)totalCells;
        if (ratio > maxRatio)
        {
            var excess = (int)(waterCount - maxRatio * totalCells);
            var rnd = RandomHelper.Instance;
            while (excess > 0)
            {
                int rx = rnd.Next(GRID_WIDTH);
                int ry = rnd.Next(GRID_HEIGHT);
                if (_grid[rx, ry] == EnvironmentType.Water)
                {
                    _grid[rx, ry] = EnvironmentType.Ground;
                    excess--;
                }
            }
        }
    }

    private void ApplySmoothingPass()
    {
        var tempGrid = (EnvironmentType[,])_grid.Clone();

        for (int x = 0; x < GRID_WIDTH; x++)
        {
            for (int y = 0; y < GRID_HEIGHT; y++)
            {
                int waterNeighbors = CountWaterNeighbors(x, y);
                tempGrid[x, y] = waterNeighbors >= 5 
                    ? EnvironmentType.Water 
                    : EnvironmentType.Ground;
            }
        }

        _grid = tempGrid;
    }

    private int CountWaterNeighbors(int cx, int cy)
    {
        int count = 0;
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                var nx = cx + dx;
                var ny = cy + dy;
                if (nx >= 0 && nx < GRID_WIDTH && ny >= 0 && ny < GRID_HEIGHT)
                {
                    if (_grid[nx, ny] == EnvironmentType.Water)
                        count++;
                }
            }
        }
        return count;
    }

    public EnvironmentType GetEnvironmentAt(int x, int y)
    {
        x = Math.Clamp(x, 0, GRID_WIDTH - 1);
        y = Math.Clamp(y, 0, GRID_HEIGHT - 1);
        return _grid[x, y];
    }
}