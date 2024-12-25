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

public class Rabbit : Herbivore
{
    public static int DefaultMaxHealth => 80;
    public static int DefaultMaxEnergy => 80;
    public override int MaxHealth => DefaultMaxHealth;
    public override int MaxEnergy => DefaultMaxEnergy;
    protected override int BaseBiteSize => 4;
   protected override double BaseBiteCooldownDuration => 0.1;
    public override double BaseHungerThreshold => 55.0;
    protected override double BaseReproductionThreshold => 70.0;
    protected override double BaseReproductionEnergyCost => 20.0;
    protected override double SpeciesEnergyCostModifier => 0.8;
    public override EnvironmentType PreferredEnvironment => EnvironmentType.Ground;
    private readonly IEntityFactory _entityFactory;


    public Rabbit(
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
            position,
            healthPoints,
            energy,
            isMale,
            visionRadius: 0.1,
            contactRadius: 0.01,
            basalMetabolicRate: 0.8)
    {
        _entityFactory = entityFactory;
        MovementSpeed = 1.5;
        Color = Brushes.Brown;

        _environmentPreferences.Clear();
        _environmentPreferences.Add(new EnvironmentPreference(PreferredEnvironment, 1.0, 1.0));

        AddBehavior(new FleeingBehavior(_worldService));        // Priority 3
        AddBehavior(new HungerBehavior());                      // Priority 2
        AddBehavior(new GroupMovementBehavior(_worldService));  // Priority 1
        AddBehavior(new RestBehavior());                        // Priority 0
        Console.WriteLine($"Created Rabbit with color {Color} at {Position.X}, {Position.Y}");
    }

    public override Animal CreateOffspring(Position position)
    {
        return _entityFactory.CreateAnimal<Rabbit>(30, 50, position, RandomHelper.Instance.NextDouble() > 0.5);
    }
}
