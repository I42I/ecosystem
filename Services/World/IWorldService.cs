using System.Collections.ObjectModel;
using ecosystem.Models.Core;
using ecosystem.Models.Entities.Environment;

namespace ecosystem.Services.World;

public interface IWorldService
{
    ObservableCollection<Entity> Entities { get; }
    GridWorld Grid { get; }
    void AddEntity(Entity entity);
    void RemoveEntity(Entity entity);
    EnvironmentType GetEnvironmentAt((double X, double Y) position);
}
