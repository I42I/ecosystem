using ecosystem.Models.Core;

namespace ecosystem.Models.Behaviors.Base;

public interface IBehavior<in T> where T : LifeForm
{
    string Name { get; }
    int Priority { get; }
    bool CanExecute(T entity);
    void Execute(T entity);
}

public class BehaviorWrapper<TSource, TTarget> : IBehavior<TTarget> 
    where TSource : TTarget
    where TTarget : LifeForm
{
    private readonly IBehavior<TSource> _behavior;
    private readonly TSource _entity;

    public BehaviorWrapper(IBehavior<TSource> behavior, TSource entity)
    {
        _behavior = behavior;
        _entity = entity;
    }

    public string Name => _behavior.Name;
    public int Priority => _behavior.Priority;

    public bool CanExecute(TTarget entity) => _behavior.CanExecute(_entity);
    public void Execute(TTarget entity) => _behavior.Execute(_entity);
}