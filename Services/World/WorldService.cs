using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using ecosystem.Models.Core;
using ecosystem.Models.Entities.Environment;
using System.Collections.Concurrent;

namespace ecosystem.Services.World;

public interface IWorldService
{
    ObservableCollection<Entity> Entities { get; }
    GridWorld Grid { get; }
    void AddEntity(Entity entity);
    void RemoveEntity(Entity entity);
    EnvironmentType GetEnvironmentAt(Position position);
    IEnumerable<Entity> GetEntitiesInRange(Position position, double radius);
    void UpdateDisplaySize(double width, double height);
    void ProcessEntityQueues();
}

public class WorldService : IWorldService
{
    private readonly object _lock = new object();
    public ObservableCollection<Entity> Entities { get; } = new();
    private readonly ConcurrentQueue<Entity> _entitiesToAdd = new();
    private readonly ConcurrentQueue<Entity> _entitiesToRemove = new();
    public GridWorld Grid { get; }

    public WorldService()
    {
        Grid = new GridWorld(800, 450);
    }

    public void AddEntity(Entity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _entitiesToAdd.Enqueue(entity);
    }

    public void RemoveEntity(Entity entity)
    {
        _entitiesToRemove.Enqueue(entity);
    }

    public void ProcessEntityQueues()
    {
        lock (_lock)
        {
            while (_entitiesToAdd.TryDequeue(out var entityToAdd))
            {
                Entities.Add(entityToAdd); 
            }

            while (_entitiesToRemove.TryDequeue(out var entityToRemove))
            {
                Entities.Remove(entityToRemove);
            }
        }
    }

    public EnvironmentType GetEnvironmentAt(Position position)
    {
        int x = (int)(position.X * Grid.Width);
        int y = (int)(position.Y * Grid.Height);
        return Grid.GetEnvironmentAt(x, y);
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

    public void UpdateDisplaySize(double width, double height)
    {
        lock (_lock)
        {
            foreach (var entity in Entities.ToList())
            {
                entity.UpdateDisplaySize(width, height);
            }
        }
    }
}