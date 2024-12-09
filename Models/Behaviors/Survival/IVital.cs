namespace ecosystem.Models.Behaviors;

public interface IVital
{
    int HealthPoints { get; }
    int Energy { get; }
    bool IsDead { get; }
    void TakeDamage(int amount);
    void ConsumeEnergy(int amount);
}
