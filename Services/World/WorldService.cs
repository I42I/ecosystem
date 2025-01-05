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
    void ProcessEntityQueues();
    bool IsValidSpawnLocation(Position position, EnvironmentType requiredEnvironment);
    void ResetGrid();
    event EventHandler GridReset;
}

public class WorldService : IWorldService
{
    private readonly object _lock = new object();
    public ObservableCollection<Entity> Entities { get; } = new();
    private readonly ConcurrentQueue<Entity> _entitiesToAdd = new();
    private readonly ConcurrentQueue<Entity> _entitiesToRemove = new();
    private GridWorld? _grid;
    public GridWorld Grid => _grid ?? throw new InvalidOperationException("Grid not yet initialized.");
    public event EventHandler? GridReset;

    public WorldService()
    {
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
            bool changes = false;
            
            while (_entitiesToRemove.TryDequeue(out var entityToRemove))
            {
                if (Entities.Remove(entityToRemove))
                {
                    changes = true;
                    Console.WriteLine($"Removed entity: {entityToRemove.GetType().Name}");
                }
            }

            while (_entitiesToAdd.TryDequeue(out var entityToAdd))
            {
                Entities.Add(entityToAdd);
                changes = true;
                Console.WriteLine($"Added entity: {entityToAdd.GetType().Name}");
            }

            if (changes)
            {
                Console.WriteLine($"Current entity count: {Entities.Count}");
            }
        }
    }

    public void ResetGrid()
    {
        _grid = new GridWorld(800, 520);
        GridReset?.Invoke(this, EventArgs.Empty);
    }

    public EnvironmentType GetEnvironmentAt(Position position)
    {
        int x = (int)(position.X * Grid.Width);
        int y = (int)(position.Y * Grid.Height);
        var environment = Grid.GetEnvironmentAt(x, y);
        
        // Console.WriteLine($"Getting environment at ({position.X},{position.Y}) -> ({x},{y}): {environment}");
        return environment;
    }

    public IEnumerable<Entity> GetEntitiesInRange(Position position, double radius)
    {
        lock (_lock)
        {
            return Entities.Where(e => GetDistance(position, e.Position) <= radius).ToList();
        }
    }

    private double GetDistance(Position pos1, Position pos2)
    {
        var dx = pos1.X - pos2.X;
        var dy = pos1.Y - pos2.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    public bool IsValidSpawnLocation(Position position, EnvironmentType requiredEnvironment)
    {
        var environment = GetEnvironmentAt(position);
        return (environment & requiredEnvironment) != 0;
    }
}