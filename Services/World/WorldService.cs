using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using ecosystem.Models.Core;
using ecosystem.Models.Entities.Environment;

namespace ecosystem.Services.World;

public class WorldService : IWorldService
{
    public ObservableCollection<Entity> Entities { get; } = new();
    public GridWorld Grid { get; }

    public WorldService()
    {
        Grid = new GridWorld(800, 450);
    }

    public void AddEntity(Entity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        // Console.WriteLine($"Adding {entity.GetType().Name} at position ({entity.Position.X}, {entity.Position.Y}) with color {entity.Color}");
        Entities.Add(entity);
        // Console.WriteLine($"Total entities: {Entities.Count}");
    }

    public void RemoveEntity(Entity entity)
    {
        Entities.Remove(entity);
    }

    public EnvironmentType GetEnvironmentAt(Position position)
    {
        return Grid.GetEnvironmentAt((int)position.X, (int)position.Y);
    }

    public IEnumerable<Entity> GetEntitiesInRange(Position position, double radius)
    {
        return Entities.Where(e => GetDistance(position, e.Position) <= radius);
    }

    private double GetDistance(Position pos1, Position pos2)
    {
        var dx = pos1.X - pos2.X;
        var dy = pos1.Y - pos2.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}