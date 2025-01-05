using System;
using ecosystem.Models.Core;
using ecosystem.Helpers;

namespace ecosystem.Models.Entities.Environment;

public class GridWorld
{
    private readonly EnvironmentType[,] _grid;
    private readonly float[,] _weights; // 0 = eau pure, 1 = terre pure
    
    private const int GRID_WIDTH = 20;
    private const int GRID_HEIGHT = 13;

    public int Width => GRID_WIDTH;
    public int Height => GRID_HEIGHT;

    public GridWorld(int displayWidth, int displayHeight)
    {
        _grid = new EnvironmentType[GRID_WIDTH, GRID_HEIGHT];
        _weights = new float[GRID_WIDTH, GRID_HEIGHT];
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        // Générer les centres des biomes
        var waterCenters = BiomeGenerator.GenerateWaterCenters(GRID_WIDTH, GRID_HEIGHT, RandomHelper.Seed);

        // Pour chaque cellule
        for (int x = 0; x < GRID_WIDTH; x++)
        {
            for (int y = 0; y < GRID_HEIGHT; y++)
            {
                // Calculer la distance au point d'eau le plus proche
                float minDistance = float.MaxValue;
                foreach (var center in waterCenters)
                {
                    float dx = x - center.x;
                    float dy = y - center.y;
                    float dist = (float)Math.Sqrt(dx * dx + dy * dy) / center.radius;
                    minDistance = Math.Min(minDistance, dist);
                }

                // Interpolation douce
                _weights[x, y] = Math.Clamp(minDistance, 0, 1);
                _grid[x, y] = _weights[x, y] < 0.5f ? EnvironmentType.Water : EnvironmentType.Ground;
            }
        }
    }

    public (EnvironmentType type, float weight) GetEnvironmentInfoAt(int x, int y)
    {
        x = Math.Clamp(x, 0, GRID_WIDTH - 1);
        y = Math.Clamp(y, 0, GRID_HEIGHT - 1);
        return (_grid[x, y], _weights[x, y]);
    }

    public EnvironmentType GetEnvironmentAt(int x, int y)
    {
        x = Math.Clamp(x, 0, GRID_WIDTH - 1);
        y = Math.Clamp(y, 0, GRID_HEIGHT - 1);
        return _grid[x, y];
    }
}