using ecosystem.Models.Core;

namespace ecosystem.Models.Behaviors.Base;

public interface IBehavior
{
    int Priority { get; }
    bool CanExecute(LifeForm entity);
    void Execute(LifeForm entity);
}