using System;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Behaviors;
using ecosystem.Models.Behaviors.Survival;

namespace ecosystem.Models.Core;

public abstract class LifeForm : Entity
{
    public int HealthPoints { get; protected set; }
    public int Energy { get; protected set; }
    public bool IsDead => HealthPoints <= 0 || Energy <= 0;

    protected LifeForm(Position position, int healthPoints, int energy)
        : base(position)
    {
        HealthPoints = healthPoints;
        Energy = energy;
    }

    public override void Update()
    {
        if (!IsDead)
        {
            UpdateBehavior();
        }
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

    protected void ConsumeEnergy(int amount)
    {
        Energy -= amount;
        if (Energy <= 0)
        {
            TakeDamage(-Energy);
            Energy = 0;
        }
    }

    protected abstract void Die();
}