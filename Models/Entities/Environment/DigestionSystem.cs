using System;
using ecosystem.Models.Core;
using ecosystem.Models.Entities.Animals;
using ecosystem.Services.World;
using ecosystem.Helpers;
using ecosystem.Services.Simulation;
using ecosystem.Models.Entities.Environment;

namespace ecosystem.Models.Entities.Environment;

public interface IDigestive
{
    void ProcessDigestion(double deltaTime);
    void AddFood(int amount);
}

public class DigestionSystem : IDigestive
{
    private double _digestionTimer;
    private int _foodInDigestion;
    private readonly Animal _animal;
    private readonly IWorldService _worldService;
    private readonly ITimeManager _timeManager;

    public DigestionSystem(Animal animal, IWorldService worldService, ITimeManager timeManager)
    {
        _animal = animal;
        _worldService = worldService;
        _timeManager = timeManager;
    }

    public void AddFood(int amount)
    {
        _foodInDigestion += amount;
        _digestionTimer = SimulationConstants.DIGESTION_TIME;
    }

    public void ProcessDigestion(double deltaTime)
    {
        if (_foodInDigestion <= 0) return;

        _digestionTimer -= deltaTime;

        if (_digestionTimer <= 0)
        {
            CreateWaste();
            _foodInDigestion = 0;
        }
    }

    private void CreateWaste()
    {
        int wasteAmount = CalculateWasteAmount();
        if (wasteAmount <= 0) return;

        var (x, y) = RandomHelper.GetRandomPositionInRadius(
            _animal.Position.X,
            _animal.Position.Y,
            SimulationConstants.WASTE_DROP_RADIUS
        );

        var waste = new OrganicWaste(
            new Position(x, y),
            wasteAmount,
            _worldService
        );

        _worldService.AddEntity(waste);
        Console.WriteLine($"{_animal.GetType().Name}#{_animal.TypeId} created waste of {wasteAmount} energy");
    }

    private int CalculateWasteAmount()
    {
        double wasteRatio = 1 - SimulationConstants.DIGESTION_EFFICIENCY;
        int wasteAmount = Math.Max(
            SimulationConstants.MIN_WASTE_PER_DIGESTION,
            (int)(_foodInDigestion * wasteRatio)
        );
        return wasteAmount;
    }
}