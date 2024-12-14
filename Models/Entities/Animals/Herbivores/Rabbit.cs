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

namespace ecosystem.Models.Entities.Animals.Herbivores;

public class Rabbit : Herbivore
{
    protected override int BaseBiteSize => 4;
    protected override double BaseHungerThreshold => 55.0;
    protected override double BaseReproductionThreshold => 70.0;
    protected override double BaseReproductionEnergyCost => 20.0;
    protected override double SpeciesEnergyCostModifier => 0.8;
    public override EnvironmentType PreferredEnvironment => EnvironmentType.Ground;

    public Rabbit(
        IEntityLocator<Animal> entityLocator,
        IEntityLocator<Plant> plantLocator,
        IWorldService worldService,
        Position position,
        int healthPoints,
        int energy,
        bool isMale)
        : base(
            entityLocator,
            plantLocator,
            worldService,
            position,
            healthPoints,
            energy,
            isMale,
            visionRadius: 8.0,
            contactRadius: 1.0,
            basalMetabolicRate: 0.8)
    {
        MovementSpeed = 1.5;
        Color = new SolidColorBrush(Colors.Brown);
        AddBehavior(new FleeingBehavior(_worldService));        // Priority 3
        AddBehavior(new HungerBehavior());                      // Priority 2
        AddBehavior(new ReproductionBehavior(_worldService));   // Priority 1
        AddBehavior(new GroupMovementBehavior(_worldService));  // Priority 1
        AddBehavior(new RestBehavior());                        // Priority 0
        Console.WriteLine($"Created Rabbit with color {Color} at {Position.X}, {Position.Y}");
    }

    public override Animal CreateOffspring(Position position)
    {
        return new Rabbit(
            _entityLocator,
            _plantLocator,
            _worldService,
            position,
            healthPoints: HealthPoints / 2,
            energy: Energy / 2,
            isMale: RandomHelper.Instance.NextDouble() > 0.5
        );
    }
}
