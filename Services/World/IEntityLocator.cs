using System.Collections.Generic;
using ecosystem.Models.Core;

namespace ecosystem.Services.World;
public interface IEntityLocator<T> where T : Entity
{
    T? FindNearest(IEnumerable<T> entities, double maxDistance, Position? fromPosition = null);
    IEnumerable<T> FindInRadius(IEnumerable<T> entities, double radius);
}
