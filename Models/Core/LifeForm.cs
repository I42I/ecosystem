using System;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Behaviors.Base;
using ecosystem.Services.Simulation;

namespace ecosystem.Models.Core;

public abstract class LifeForm : Entity
{
    protected readonly ITimeManager _timeManager;
    private double _energyAccumulator;
    private double _healthAccumulator;
    private int _healthPoints;
    private int _energy;
    public bool IsDead => HealthPoints <= 0 || Energy <= 0;
    protected EnvironmentType Environment { get; }

    protected LifeForm(
        Position position, 
        int healthPoints, 
        int energy, 
        EnvironmentType environment,
        ITimeManager timeManager)
        : base(position)
    {
        _timeManager = timeManager;
        HealthPoints = healthPoints;
        Energy = energy;
        Environment = environment;
    }

    public int HealthPoints 
    { 
        get => _healthPoints;
        protected set
        {
            if (_healthPoints != value)
            {
                _healthPoints = value;
                OnPropertyChanged(nameof(HealthPoints));
            }
        }
    }

    public int Energy 
    { 
        get => _energy;
        protected set
        {
            if (_energy != value)
            {
                _energy = value;
                OnPropertyChanged(nameof(Energy));
            }
        }
    }

    public override void Update()
    {
        if (!IsDead)
        {
            ConsumeEnergy(SimulationConstants.BASE_ENERGY_LOSS);
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
            // Console.WriteLine($"[{GetType().Name}] Current behavior: {behavior?.Name ?? "None"}");
            
            if (behavior != null)
            {
                // Console.WriteLine($"Executing behavior {behavior.Name} for {GetType().Name}");
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

    public void TakeDamage(double amount)
    {
        _healthAccumulator += amount;
        
        if (_healthAccumulator >= 1)
        {
            int damageToApply = (int)Math.Floor(_healthAccumulator);
            HealthPoints = Math.Max(0, HealthPoints - damageToApply);
            _healthAccumulator -= damageToApply;

            Console.WriteLine($"{GetType().Name} took {damageToApply} damage, HP: {HealthPoints}");

            if (HealthPoints <= 0)
            {
                Die();
            }
        }
    }

    protected void ConsumeEnergy(double amount)
    {
        _energyAccumulator += amount;
        
        if (_energyAccumulator >= 1)
        {
            int energyToConsume = (int)Math.Floor(_energyAccumulator);
            Energy = Math.Max(0, Energy - energyToConsume);
            _energyAccumulator -= energyToConsume;

            if (Energy <= 0)
            {
                TakeDamage(1);
                Energy = 0;
            }
        }
    }
        
    protected abstract void Die();
}