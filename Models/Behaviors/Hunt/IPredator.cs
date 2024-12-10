using ecosystem.Models.Entities.Animals;

namespace ecosystem.Models.Behaviors.Hunt;

public interface IPredator
{
    Animal? FindNearestPrey();
    void MoveTowardsPrey(Animal prey);
    bool CanAttack(Animal prey);
    void Attack(Animal prey);
}
