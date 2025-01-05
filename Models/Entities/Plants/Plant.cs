using System;
using System.Linq;
using System.Collections.Generic;
using ecosystem.Helpers;
using ecosystem.Models.Core;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Behaviors;
using ecosystem.Services.World;
using ecosystem.Services.Simulation;
using ecosystem.Models.Radius;

namespace ecosystem.Models.Entities.Plants;

public abstract class Plant : LifeForm, IHasRootSystem
{
    private double _growthAccumulator;
    private double _reproductionAccumulator;
    protected bool _hasCreatedWaste = false;
    protected abstract double BaseAbsorptionRate { get; }
    protected readonly IWorldService _worldService;
    public abstract EnvironmentType PreferredEnvironment { get; }
    public double RootRadius { get; protected set; }
    public double SeedRadius { get; protected set; }

    private const double BASE_HEALING_COST = 2.0;
    private const double RADIUS_GROWTH_RATE = 0.003;
    private readonly double _baseContactRadius;

    protected Plant(
        int healthPoints,
        int energy,
        Position position,
        double basalMetabolicRate,
        EnvironmentType environment,
        double rootRadius,
        double seedRadius,
        double contactRadius,
        IWorldService worldService,
        ITimeManager timeManager)
        : base(position, healthPoints, energy, environment, timeManager)
    {
        _baseContactRadius = contactRadius;
        ContactRadius = contactRadius;
        _worldService = worldService;
        RootRadius = rootRadius;
        SeedRadius = seedRadius;
        ContactRadius = contactRadius;
    }

    private double _absorptionCooldown = 0;
    protected override void UpdateBehavior()
    {
        base.UpdateBehavior();

        if (Energy > SimulationConstants.HEALING_ENERGY_THRESHOLD/100 * MaxEnergy)
        {
            double excessEnergy = Energy - (SimulationConstants.HEALING_ENERGY_THRESHOLD/100 * MaxEnergy);
            double energyToConvert = Math.Min(5, excessEnergy * 0.1);
            
            if (energyToConvert >= 1)
            {
                ConvertEnergyToHealth(energyToConvert);
            }
        }

        UpdateContactRadius();

        ConsumeEnergy(SimulationConstants.BASE_ENERGY_LOSS * _timeManager.DeltaTime);

        if (_absorptionCooldown > 0)
        {
            _absorptionCooldown -= _timeManager.DeltaTime;
        }

        var nearbyWaste = _worldService.GetEntitiesInRange(Position, RootRadius)
            .OfType<OrganicWaste>()
            .ToList();

        foreach (var waste in nearbyWaste)
        {
            if (_absorptionCooldown <= 0)
            {
                AbsorbWaste(waste);
                _absorptionCooldown = 0.2;
            }
        }

        _growthAccumulator += _timeManager.DeltaTime;
        if (_growthAccumulator >= SimulationConstants.PLANT_GROWTH_INTERVAL)
        {
            ProcessGrowth();
            _growthAccumulator = 0;
        }

        _reproductionAccumulator += _timeManager.DeltaTime;
        if (_reproductionAccumulator >= SimulationConstants.PLANT_REPRODUCTION_CHECK_INTERVAL)
        {
            double healthFactor = (double)HealthPoints / MaxHealth;
            double energyFactor = (double)Energy / MaxEnergy;
            double reproductionChance = SimulationConstants.PLANT_SEED_SPREAD_BASE_CHANCE * 
                                    healthFactor * 
                                    energyFactor;

            if (CanSpreadSeeds() && RandomHelper.Instance.NextDouble() < reproductionChance)
            {
                SpreadSeeds();
            }
            _reproductionAccumulator = 0;
        }
    }

    private void UpdateContactRadius()
    {
        if (HealthPoints > MaxHealth)
        {
            double healthExcess = HealthPoints - MaxHealth;
            double growthFactor = Math.Log10(1 + healthExcess) * RADIUS_GROWTH_RATE;
            ContactRadius = _baseContactRadius + growthFactor;
        }
        else
        {
            ContactRadius = _baseContactRadius;
        }
    }

    protected void ConvertEnergyToHealth(double energyAmount)
    {
        if (energyAmount <= 0) return;

        int energyToConvert = (int)Math.Ceiling(energyAmount);
        int healingAmount = (int)(energyToConvert / BASE_HEALING_COST);

        if (healingAmount > 0)
        {
            Energy -= energyToConvert;
            HealthPoints += healingAmount;
                
            Console.WriteLine($"[{GetType().Name}#{TypeId}] Converted {energyToConvert} energy to {healingAmount} health. New HP: {HealthPoints}");
        }
    }

    protected virtual void ProcessGrowth()
    {
        if (Environment.HasFlag(PreferredEnvironment))
        {
            Energy += 1;
        }
    }

    protected virtual void AbsorbWaste(OrganicWaste waste)
    {
        // Absorption plus significative
        double absorbedEnergy = waste.EnergyValue * BaseAbsorptionRate;
        
        Console.WriteLine($"[{GetType().Name}#{TypeId}] attempting to absorb {absorbedEnergy} energy from waste with value {waste.EnergyValue}");

        if (absorbedEnergy >= SimulationConstants.WASTE_ABSORPTION_THRESHOLD)
        {
            // Absorber une partie significative de l'énergie
            int energyToAbsorb = (int)Math.Ceiling(absorbedEnergy);
            int previousEnergy = Energy;
            Energy += energyToAbsorb;
            waste.EnergyValue = Math.Max(0, waste.EnergyValue - energyToAbsorb);

            // Croissance plus significative
            double growthFactor = energyToAbsorb * SimulationConstants.ROOT_GROWTH_RATE;
            double previousRadius = RootRadius;
            RootRadius += growthFactor;
            RootRadius = Math.Min(RootRadius, SimulationConstants.MAX_ROOT_RADIUS);

            Console.WriteLine($"[{GetType().Name}#{TypeId}]: Energy {previousEnergy}->{Energy}, Radius {previousRadius:F3}->{RootRadius:F3}");

            // Si le déchet n'a plus d'énergie, le supprimer
            if (waste.EnergyValue <= 0)
            {
                _worldService.RemoveEntity(waste);
            }
        }
    }

    protected virtual bool CanSpreadSeeds()
    {
        return Energy >= SimulationConstants.PLANT_MIN_ENERGY_FOR_REPRODUCTION && 
            HealthPoints >= SimulationConstants.PLANT_MIN_HEALTH_FOR_REPRODUCTION;
    }

    protected abstract Plant CreateOffspring(Position position);

    private void SpreadSeeds()
    {
        int maxAttempts = 10;
        for (int i = 0; i < maxAttempts; i++)
        {
            var position = RandomHelper.GetRandomPositionInRadiusForEnvironment(
                Position.X,
                Position.Y,
                SeedRadius,
                PreferredEnvironment,
                _worldService
            );

            if (_worldService.IsValidSpawnLocation(position, PreferredEnvironment))
            {
                var offspring = CreateOffspring(position);
                Energy -= (int)SimulationConstants.PLANT_REPRODUCTION_ENERGY_COST;
                _worldService.AddEntity(offspring);
                Console.WriteLine($"{GetType().Name}#{TypeId} spread seeds at ({position.X:F2}, {position.Y:F2})");
                return;
            }
        }
    }

    protected override void Die()
    {
        if (_hasCreatedWaste) return;
        _hasCreatedWaste = true;

        var waste = new OrganicWaste(Position, Energy, _worldService);
        _worldService.AddEntity(waste);
        _worldService.RemoveEntity(this);
    }
}
