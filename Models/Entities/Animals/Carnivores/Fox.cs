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

namespace ecosystem.Models.Entities.Animals.Carnivores;

public class Fox : Carnivore
{
    public override double BaseAttackPower => 15.0;
    protected override double BaseAttackRange => 1.5;
    protected override double BaseHungerThreshold => 40.0;
    protected override double BaseReproductionThreshold => 60.0;
    protected override double BaseReproductionEnergyCost => 30.0;
    protected override double SpeciesEnergyCostModifier => 1.2;
    public override EnvironmentType PreferredEnvironment => EnvironmentType.Ground;
    private readonly Position _territoryCenter;

    public Fox(
        IEntityLocator<Animal> entityLocator,
        IEntityLocator<Animal> preyLocator,
        IWorldService worldService,
        ITimeManager timeManager,
        Position position,
        int healthPoints,
        int energy,
        bool isMale)
        : base(
            entityLocator,
            preyLocator,
            worldService,
            timeManager,
            position,
            healthPoints,
            energy,
            isMale,
            visionRadius: 10.0,
            contactRadius: 2.0,
            basalMetabolicRate: 1.2)
    {
        MovementSpeed = 2.0;
        Color = new SolidColorBrush(Colors.Red);
        _territoryCenter = position;
        
        AddBehavior(new HuntingBehavior(worldService, new GroundHuntingStrategy()));
        AddBehavior(new TerritorialBehavior(worldService, position));
        Console.WriteLine($"Created Fox with color {Color} at {Position.X}, {Position.Y}");
        
    }

    public override Animal CreateOffspring(Position position)
    {
        return new Fox(
            _entityLocator,
            _preyLocator,
            _worldService,
            _timeManager,
            position,
            HealthPoints / 2,
            Energy / 2,
            RandomHelper.Instance.NextDouble() > 0.5);
    }
}

