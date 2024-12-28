using System;
using System.Collections.Generic;
using System.Linq;
using ecosystem.Models.Core;

namespace ecosystem.Services.World;

public class WorldEntityLocator<T> : IEntityLocator<T> where T : Entity
{
    private readonly IWorldService _worldService;

    public WorldEntityLocator(IWorldService worldService)
    {
        _worldService = worldService;
    }

    public T? FindNearest(IEnumerable<T> entities, double maxDistance, Position? fromPosition = null)
    {
        var referencePos = fromPosition ?? new Position(0, 0);
        
        return entities
            .OrderBy(e => GetDistance(referencePos, e.Position))
            .FirstOrDefault(e => GetDistance(referencePos, e.Position) <= maxDistance);
    }

    public IEnumerable<T> FindInRadius(IEnumerable<T> entities, double radius)
    {
        return entities.Where(e => GetDistance(e.Position, new Position(0, 0)) <= radius);
    }

    private double GetDistance(Position pos1, Position pos2)
    {
        var dx = pos1.X - pos2.X;
        var dy = pos1.Y - pos2.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}