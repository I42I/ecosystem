using System;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Behaviors.Base;
using ecosystem.Services.Simulation;
using ecosystem.Models.Radius;

namespace ecosystem.Models.Core;

public abstract class LifeForm : Entity, IHasContactRange
{
    protected readonly ITimeManager _timeManager;
    private double _energyAccumulator;
    private double _healthAccumulator;
    private int _healthPoints;
    private int _energy;
    public abstract int MaxHealth { get; }
    public abstract int MaxEnergy { get; }
    public bool IsDead => HealthPoints <= 0 || Energy <= 0;
    protected EnvironmentType Environment { get; }
    public double ContactRadius { get; protected set; }

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
                return;
            }

            if (HealthPoints < 10 && Energy > 20)
            {
                int energyToConvert = Math.Min(20, Energy);
                Energy -= energyToConvert;
                HealthPoints += energyToConvert / HEALTH_TO_ENERGY_CONVERSION_RATE;
                Console.WriteLine($"{GetType().Name} converted {energyToConvert} energy to {energyToConvert / HEALTH_TO_ENERGY_CONVERSION_RATE} HP");
            }
        }
    }

    protected const int HEALTH_TO_ENERGY_CONVERSION_RATE = 10;

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
                int healthToConvert = Math.Min(
                    HealthPoints - 1,
                    (int)Math.Ceiling(10.0 / HEALTH_TO_ENERGY_CONVERSION_RATE)
                );

                if (healthToConvert > 0)
                {
                    HealthPoints -= healthToConvert;
                    Energy += healthToConvert * HEALTH_TO_ENERGY_CONVERSION_RATE;
                    Console.WriteLine($"{GetType().Name} converted {healthToConvert} HP to {healthToConvert * HEALTH_TO_ENERGY_CONVERSION_RATE} energy");
                }
                else
                {
                    TakeDamage(SimulationConstants.HEALTH_LOSS_WHEN_STARVING);
                }
            }
        }
    }
        
    protected abstract void Die();
}