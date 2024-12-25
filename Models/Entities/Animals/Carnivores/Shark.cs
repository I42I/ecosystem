using Avalonia.Media;
using ecosystem.Models.Entities.Environment;
using ecosystem.Helpers;
using System.Collections.Generic;
using System.Linq;
using ecosystem.Models.Behaviors;
using ecosystem.Models.Core;
using ecosystem.Services.World;
using ecosystem.Services.Simulation;
using ecosystem.Services.Factory;

namespace ecosystem.Models.Entities.Animals.Carnivores;

public class Shark : Carnivore
{

    public static int DefaultMaxHealth => 150;
    public static int DefaultMaxEnergy => 200;
    public override int MaxHealth => DefaultMaxHealth;
    public override int MaxEnergy => DefaultMaxEnergy;
    public override double BaseAttackPower => 25.0;
    protected override double BaseAttackRange => 2.0;
    protected override double BaseBiteCooldownDuration => 1.0;
    protected override double BaseHungerThreshold => 50.0;
    protected override double BaseReproductionThreshold => 70.0;
    protected override double BaseReproductionEnergyCost => 40.0;
    protected override double SpeciesEnergyCostModifier => 1.5;
    public override EnvironmentType PreferredEnvironment => EnvironmentType.Water;
    private readonly IEntityFactory _entityFactory;

    public Shark(
        IEntityLocator<Animal> entityLocator,
        IEntityLocator<Animal> preyLocator,
        IWorldService worldService,
        ITimeManager timeManager,
        IEntityFactory entityFactory,
        int healthPoints,
        int energy,
        Position position,
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
            visionRadius: 15.0,
            contactRadius: 3.0,
            basalMetabolicRate: 1.5)
    {
        _entityFactory = entityFactory;
        Color = Brushes.Gray;
    }

    public override Animal CreateOffspring(Position position)
    {
        return _entityFactory.CreateAnimal<Shark>(30, 50, position, RandomHelper.Instance.NextDouble() > 0.5);
    }

    // protected override double GetEnvironmentMovementModifier()
    // {
    //     return Environment.HasFlag(PreferredEnvironment) ? 0.8 : 2.0;
    // }
}
