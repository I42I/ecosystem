using System.Collections.Generic;
using Avalonia.Media;

namespace ecosystem.Models.Entities.Environment;

public class GridWorld
{
    public int Width { get; }
    public int Height { get; }
    private readonly EnvironmentType[,] grid;
    private readonly Dictionary<EnvironmentType, IBrush> environmentColors;

    public GridWorld(int width, int height)
    {
        Width = width;
        Height = height;
        grid = new EnvironmentType[width, height];

        environmentColors = new Dictionary<EnvironmentType, IBrush>
        {
            { EnvironmentType.Ground, new SolidColorBrush(Colors.SandyBrown) },
            { EnvironmentType.Water, new SolidColorBrush(Colors.LightBlue) },
            { EnvironmentType.Ground | EnvironmentType.Water, new SolidColorBrush(Colors.LightGreen) }
        };
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        int halfWidth = Width / 2;
        int halfHeight = Height / 2;

        FillZone(0, 0, halfWidth, halfHeight, EnvironmentType.Ground);
        
        FillZone(halfWidth, 0, Width, halfHeight, EnvironmentType.Water);
        
        FillZone(0, halfHeight, halfWidth, Height, EnvironmentType.Ground | EnvironmentType.Water);
        
        FillZone(halfWidth, halfHeight, Width, Height, EnvironmentType.Ground);
    }

    private void FillZone(int startX, int startY, int endX, int endY, EnvironmentType type)
    {
        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                grid[x, y] = type;
            }
        }
    }

    public EnvironmentType GetEnvironmentAt(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return EnvironmentType.None;
        return grid[x, y];
    }

    public IBrush GetEnvironmentColorAt(int x, int y)
    {
        var environment = GetEnvironmentAt(x, y);
        return environmentColors[environment];
    }
}
