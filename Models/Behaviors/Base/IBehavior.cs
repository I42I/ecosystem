using ecosystem.Models.Core;

namespace ecosystem.Models.Behaviors.Base;

public interface IBehavior<T> where T : LifeForm
{
    int Priority { get; }
    bool CanExecute(T entity);
    void Execute(T entity);
}