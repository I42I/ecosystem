using System;
using System.Collections.Generic;
using ecosystem.Helpers;
using ecosystem.Models.Core;
using ecosystem.Models.Entities.Environment;
using ecosystem.Models.Behaviors;

namespace ecosystem.Models.Entities.Plants;

public abstract class Plant : LifeForm
{
    protected Plant(
        int healthPoints,
        int energy,
        Position position,
        double basalMetabolicRate,
        EnvironmentType environment)
        : base(healthPoints, energy, position, basalMetabolicRate, environment)
    {
    }

    public double RootRadius { get; protected set; }
    public double SeedRadius { get; protected set; }

    protected override void UpdateBehavior()
    {
        ConsumeOrganicWaste();
        if (CanSpreadSeeds())
        {
            SpreadSeeds();
        }
    }

    private void ConsumeOrganicWaste()
    {
        List<OrganicWaste> wastes = FindOrganicWasteInRadius(RootRadius);
        foreach (var waste in wastes)
        {
            AbsorbWaste(waste);
        }
    }

    private List<OrganicWaste> FindOrganicWasteInRadius(double radius)
    {
        return new List<OrganicWaste>();
    }

    private void AbsorbWaste(OrganicWaste waste)
    {
        Energy += waste.EnergyValue;
    }

    protected abstract bool CanSpreadSeeds();
    protected abstract Plant CreateOffspring(Position position);

    private void SpreadSeeds()
    {
        var (x, y) = RandomHelper.GetRandomPositionInRadius(Position.X, Position.Y, SeedRadius);
        var randomPosition = new Position(x, y);
        var offspring = CreateOffspring(randomPosition);
    }

    protected override void OnDeath()
    {
        CreateOrganicWaste();
    }

    private void CreateOrganicWaste()
    {
        // Instantiate an OrganicWaste object at the plant's position
        // OrganicWaste waste = new OrganicWaste(Position, Energy);
        // Add waste to the ecosystem
    }

    public double MovementSpeed => 0.0;

    public void Move(double deltaX, double deltaY)
    {
        // Les plantes ne peuvent pas se déplacer
    }

    public double GetDistanceTo(IMoveable other)
    {
        double deltaX = Position.X - other.Position.X;
        double deltaY = Position.Y - other.Position.Y;
        return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }
}
