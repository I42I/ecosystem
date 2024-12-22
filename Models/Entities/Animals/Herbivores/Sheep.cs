using ecosystem.Helpers;
using ecosystem.Models.Behaviors;
using ecosystem.Models.Behaviors.Base;
using ecosystem.Models.Behaviors.Movement;
using ecosystem.Models.Behaviors.Reproduction;
using ecosystem.Models.Behaviors.Survival;
using ecosystem.Models.Core;
using ecosystem.Services.World;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Entities.Plants;
using Avalonia.Media;
using ecosystem.Services.Simulation;

namespace ecosystem.Models.Entities.Animals.Herbivores;

public class Sheep : Herbivore
{
    protected override int BaseBiteSize => 10;
    protected override double BiteCooldownDuration => 3.0;
    protected override double BaseHungerThreshold => 45.0;
    protected override double BaseReproductionThreshold => 75.0;
    protected override double BaseReproductionEnergyCost => 40.0;
    protected override double SpeciesEnergyCostModifier => 1.0;
    public override EnvironmentType PreferredEnvironment => EnvironmentType.Ground;
    public Sheep(
        IEntityLocator<Animal> entityLocator,
        IEntityLocator<Plant> plantLocator, 
        IWorldService worldService,
        ITimeManager timeManager,
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
            visionRadius: 8.0,
            contactRadius: 1.5,
            basalMetabolicRate: 0.7)
    {
        Color = Brushes.White;;
        AddBehavior(new FleeingBehavior(_worldService));        // Priority 3
        AddBehavior(new HungerBehavior());                      // Priority 2
        AddBehavior(new GroupMovementBehavior(_worldService));  // Priority 1
        AddBehavior(new RestBehavior());                        // Priority 0

        MovementSpeed = 1.3;
    }

    public override Animal CreateOffspring(Position position)
    {
        return new Sheep(
            _entityLocator,
            _plantLocator,
            _worldService,
            _timeManager,
            position,
            healthPoints: HealthPoints / 2,
            energy: Energy / 2,
            isMale: RandomHelper.Instance.NextDouble() > 0.5
        );
    }
}