using System;
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
}