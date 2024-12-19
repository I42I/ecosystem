using System;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Behaviors.Base;

namespace ecosystem.Models.Core;

public abstract class LifeForm : Entity
{
    public int HealthPoints { get; protected set; }
    public int Energy { get; protected set; }
    public bool IsDead => HealthPoints <= 0 || Energy <= 0;
    protected EnvironmentType Environment { get; }

    protected LifeForm(Position position, int healthPoints, int energy, EnvironmentType environment)
        : base(position)
    {
        HealthPoints = healthPoints;
        Energy = energy;
        Environment = environment;
    }

    public override void Update()
    {
        if (!IsDead)
        {
            UpdateBehavior();
        }
        else
        {
            Die();
        }
    }

    protected virtual IBehavior<LifeForm>? GetCurrentBehavior()
    {
        return null;
    }

    protected virtual void UpdateBehavior()
    {
        if (!IsDead)
        {
            var behavior = GetCurrentBehavior();
            Console.WriteLine($"[{GetType().Name}] Current behavior: {behavior?.Name ?? "None"}");
            
            if (behavior != null)
            {
                Console.WriteLine($"Executing behavior {behavior.Name} for {GetType().Name}");
                Stats.CurrentBehavior = behavior.Name;
                behavior.Execute(this as dynamic);
            }
            else
            {
                Console.WriteLine($"No behavior found for {GetType().Name}");
            }
        }
        else
        {
            Stats.CurrentBehavior = "Dead";
            Die();
        }
    }

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