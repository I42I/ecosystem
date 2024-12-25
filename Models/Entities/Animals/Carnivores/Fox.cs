using System;
using Avalonia.Media;
using ecosystem.Helpers;
using ecosystem.Models.Behaviors;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Behaviors.Hunt;
using ecosystem.Services.World;
using ecosystem.Models.Core;
using ecosystem.Models.Behaviors.Movement;
using ecosystem.Services.Simulation;
using ecosystem.Services.Factory;

namespace ecosystem.Models.Entities.Animals.Carnivores;

public class Fox : Carnivore
{
    public static int DefaultMaxHealth => 100;
    public static int DefaultMaxEnergy => 100;
    public override int MaxHealth => DefaultMaxHealth;
    public override int MaxEnergy => DefaultMaxEnergy;
    public override double BaseAttackPower => 15.0;
    protected override double BaseAttackRange => 1.5;
    protected override double BaseBiteCooldownDuration => 0.03;
    public override double BaseHungerThreshold => 40.0;
    protected override double BaseReproductionThreshold => 60.0;
    protected override double BaseReproductionEnergyCost => 30.0;
    protected override double SpeciesEnergyCostModifier => 1.2;
    public override EnvironmentType PreferredEnvironment => EnvironmentType.Ground;
    private readonly Position _territoryCenter;
    private readonly IEntityFactory _entityFactory;

    public Fox(
        IEntityLocator<Animal> entityLocator,
        IEntityLocator<Animal> preyLocator,
        IWorldService worldService,
        ITimeManager timeManager,
        IEntityFactory entityFactory,
        Position position,
        int healthPoints,
        int energy,
        bool isMale)
        : base(
            entityLocator,
            preyLocator,
            worldService,
            timeManager,
            entityFactory,
            position,
            healthPoints,
            energy,
            isMale,
            visionRadius: 0.15,
            contactRadius: 0.01,
            basalMetabolicRate: 1.2)
    {
        _entityFactory = entityFactory;
        MovementSpeed = 2.0;
        Color = Brushes.Orange;
        _territoryCenter = position;

        _environmentPreferences.Clear();
        _environmentPreferences.Add(new EnvironmentPreference(PreferredEnvironment, 1.0, 1.0));
        
        AddBehavior(new HuntingBehavior(worldService, new GroundHuntingStrategy(worldService)));
        // AddBehavior(new TerritorialBehavior(worldService, position));
        Console.WriteLine($"Created Fox with color {Color} at {Position.X}, {Position.Y}");
        
    }

    public override Animal CreateOffspring(Position position)
    {
        return _entityFactory.CreateAnimal<Fox>(30, 50, position, RandomHelper.Instance.NextDouble() > 0.5);
    }
}

