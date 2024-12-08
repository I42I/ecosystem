using System.Collections.Generic;
using ecosystem.Models.Core;

namespace ecosystem.Models.Behaviors;

public interface IEntityLocator<T> where T : Entity
{
    T? FindNearest(IEnumerable<T> entities, double maxDistance);
    IEnumerable<T> FindInRadius(IEnumerable<T> entities, double radius);
}
