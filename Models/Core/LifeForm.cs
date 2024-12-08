using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Behaviors;

namespace ecosystem.Models.Core;

public abstract class LifeForm : Entity, IVital, IEnvironmentSensitive
{
    public int HealthPoints { get; protected set; }
    public int Energy { get; protected set; }
    public double BasalMetabolicRate { get; protected set; }
    public bool IsDead { get; protected set; }
    public EnvironmentType Environment { get; protected set; }

    public abstract EnvironmentType PreferredEnvironment { get; }

    public virtual double GetEnvironmentMovementModifier()
    {
        return Environment.HasFlag(PreferredEnvironment) ? 1.0 : 1.5;
    }

    protected LifeForm(
        int healthPoints, 
        int energy, 
        (double X, double Y) position,
        double basalMetabolicRate,
        EnvironmentType environment) : base(position)
    {
        HealthPoints = healthPoints;
        Energy = energy;
        BasalMetabolicRate = basalMetabolicRate;
        Environment = environment;
    }

    public override void Update()
    {
        if (IsDead) return;
        
        ConsumeEnergy((int)BasalMetabolicRate);
        UpdateBehavior();
    }

    protected abstract void UpdateBehavior();

    public void TakeDamage(int amount)
    {
        HealthPoints -= amount;
        if (HealthPoints <= 0)
        {
            Die();
        }
    }

    public void ConsumeEnergy(int amount)
    {
        InternalConsumeEnergy(amount);
    }

    protected void InternalConsumeEnergy(int amount)
    {
        Energy -= amount;
        if (Energy <= 0)
        {
            ConvertHealthToEnergy();
        }
    }

    protected void ConvertHealthToEnergy()
    {
        if (HealthPoints + Energy > 0)
        {
            HealthPoints += Energy;
            Energy = 0;
        }
        else
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        IsDead = true;
        OnDeath();
    }

    protected abstract void OnDeath();
}