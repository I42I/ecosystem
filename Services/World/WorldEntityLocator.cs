using System;
using System.Collections.Generic;
using System.Linq;
using ecosystem.Models.Core;
using ecosystem.Models.Behaviors;

namespace ecosystem.Services.World;

public class WorldEntityLocator<T> : IEntityLocator<T> where T : Entity
{
    private readonly IWorldService _worldService;

    public WorldEntityLocator(IWorldService worldService)
    {
        _worldService = worldService;
    }

    public T? FindNearest(IEnumerable<T> entities, double maxDistance)
    {
        return entities
            .OrderBy(e => GetDistance(e.Position))
            .FirstOrDefault(e => GetDistance(e.Position) <= maxDistance);
    }

    public IEnumerable<T> FindInRadius(IEnumerable<T> entities, double radius)
    {
        return entities.Where(e => GetDistance(e.Position) <= radius);
    }

    private double GetDistance((double X, double Y) position)
    {
        return Math.Sqrt(position.X * position.X + position.Y * position.Y);
    }
}
