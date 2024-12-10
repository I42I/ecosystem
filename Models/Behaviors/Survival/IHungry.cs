namespace ecosystem.Models.Behaviors.Survival;

public interface IHungry
{
    double HungerThreshold { get; }
    void SearchForFood();
}