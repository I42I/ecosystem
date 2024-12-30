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

public class Cheetah : Carnivore
{
    public static int DefaultMaxHealth => 70;
    public static int DefaultMaxEnergy => 120;
    public override int MaxHealth => DefaultMaxHealth;
    public override int MaxEnergy => DefaultMaxEnergy;
    public override double BaseAttackPower => 20.0;
    protected override double BaseAttackRange => 1.2;
    protected override double BaseBiteCooldownDuration => 0.03;
    public override double BaseHungerThreshold => 30.0;
    protected override double BaseReproductionThreshold => 60.0;
    protected override double BaseReproductionEnergyCost => 30.0;
    protected override double SpeciesEnergyCostModifier => 1.2;
    public override EnvironmentType PreferredEnvironment => EnvironmentType.Ground;    private readonly Position _territoryCenter;
    private readonly IEntityFactory _entityFactory;

    public Cheetah(

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
            visionRadius: 10.0,
            contactRadius: 2.0,
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
        Console.WriteLine($"Created Cheetah with color {Color} at {Position.X}, {Position.Y}");
        
    }

    public override Animal CreateOffspring(Position position)
    {
        return _entityFactory.CreateAnimal<Cheetah>(30, 50, position, RandomHelper.Instance.NextDouble() > 0.5);
    }
}

