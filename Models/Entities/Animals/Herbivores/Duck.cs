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

namespace ecosystem.Models.Entities.Animals.Herbivores;

public class Duck : Herbivore
{
    protected override int BaseBiteSize => 5;
    protected override double BiteCooldownDuration => 1.5;
    public override double BaseHungerThreshold => 50.0;
    protected override double BaseReproductionThreshold => 65.0;
    protected override double BaseReproductionEnergyCost => 30.0;
    protected override double SpeciesEnergyCostModifier => 0.7;
    public override EnvironmentType PreferredEnvironment => EnvironmentType.Ground | EnvironmentType.Water;

    public Duck(
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
        return new Duck(
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