using System.Collections.Generic;
using ecosystem.Models.Core;

namespace ecosystem.Services.World;

public interface IEntityManager
{
    void AddEntity(Entity entity);
    void RemoveEntity(Entity entity);
    IEnumerable<T> GetNearbyEntities<T>(Entity source, double radius) where T : Entity;
    void Update();
}
