using System;
using System.Collections.Generic;
using System.Linq;
using ecosystem.Models.Core;
using ecosystem.Models.Behaviors;

namespace ecosystem.Services.World;

public class EntityManager : IEntityManager
{
    private readonly List<Entity> _entities = new();
    private readonly IWorldService _worldService;

    public EntityManager(IWorldService worldService)
    {
        _worldService = worldService;
    }

    public void AddEntity(Entity entity)
    {
        _entities.Add(entity);
    }

    public void RemoveEntity(Entity entity)
    {
        _entities.Remove(entity);
    }

    public IEnumerable<T> GetNearbyEntities<T>(Entity source, double radius) where T : Entity
    {
        return _entities
            .OfType<T>()
            .Where(e => CalculateDistance(source.Position, e.Position) <= radius);
    }

    private double CalculateDistance(Position pos1, Position pos2)
    {
        var dx = pos1.X - pos2.X;
        var dy = pos1.Y - pos2.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    public void Update()
    {
        foreach (var entity in _entities.ToList())
        {
            entity.Update();
        }
    }
}
