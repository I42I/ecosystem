using System;
using Avalonia.Media;
using System.Linq;
using ecosystem.Models.Entities.Environment;
using ecosystem.Helpers;
using ecosystem.Models.Behaviors;
using ecosystem.Models.Entities.Plants;
using ecosystem.Services.World;
using ecosystem.Models.Core;
using ecosystem.Models.Behaviors.Survival;
using ecosystem.Models.Behaviors.Movement;
using ecosystem.Models.Behaviors.Reproduction;
using ecosystem.Models.Behaviors.Base;
using ecosystem.Services.Simulation;
using ecosystem.Services.Factory;

namespace ecosystem.Models.Entities.Animals.Herbivores;

public class Antelope : Herbivore
{
    public static int DefaultMaxHealth => 80;
    public static int DefaultMaxEnergy => 80;
    public override int MaxHealth => DefaultMaxHealth;
    public override int MaxEnergy => DefaultMaxEnergy;
    protected override int BaseBiteSize => 5;
    protected override double BaseBiteCooldownDuration => 0.02;
    public override double BaseHungerThreshold => 50.0;
    protected override double BaseReproductionThreshold => 65.0;
    protected override double BaseReproductionEnergyCost => 30.0;
    protected override double SpeciesEnergyCostModifier => 0.7;
    public override EnvironmentType PreferredEnvironment => EnvironmentType.Ground | EnvironmentType.Water;
    private readonly IEntityFactory _entityFactory;

    public Antelope(
        IEntityLocator<Animal> entityLocator,
        IEntityLocator<Plant> plantLocator,
        IWorldService worldService,
        ITimeManager timeManager,
        IEntityFactory entityFactory,
        Position position,
        int healthPoints,
        int energy,
        bool isMale)
        : base(
            entityLocator,
            plantLocator,
            worldService,
            timeManager,
            entityFactory,
            position,
            healthPoints,
            energy,
            isMale,
            visionRadius: 8.0,
            contactRadius: 1.5,
            basalMetabolicRate: 0.7)
    {
        _entityFactory = entityFactory;
        _environmentPreferences.AddRange(new[]
        {
            new EnvironmentPreference(EnvironmentType.Ground, 0.8, 1.2),
            new EnvironmentPreference(EnvironmentType.Water, 1.0, 1.0),
        });
        
        AddBehavior(new FleeingBehavior(worldService));
        AddBehavior(new BirthBehavior());
        AddBehavior(new EnvironmentSeekingBehavior(worldService));
        AddBehavior(new HungerBehavior());
        AddBehavior(new RestBehavior());
        
        MovementSpeed = 1.4;
        Color = Brushes.Yellow;
    }

    public override Animal CreateOffspring(Position position)
    {
        return _entityFactory.CreateAnimal<Antelope>(30, 50, position, RandomHelper.Instance.NextDouble() > 0.5);
    }
}