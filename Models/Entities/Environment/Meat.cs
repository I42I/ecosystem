using Avalonia.Media;
using ecosystem.Models.Core;
using ecosystem.Services.Simulation;
using ecosystem.Services.World;

namespace ecosystem.Models.Entities.Environment;

public class Meat : LifeForm
{
    public static int DefaultMaxHealth => 100;
    public static int DefaultMaxEnergy => 200;
    public override int MaxHealth => DefaultMaxHealth;
    public override int MaxEnergy => DefaultMaxEnergy;
    private readonly IWorldService _worldService;

    public Meat(Position position, int healthValue, int energyValue, ITimeManager timeManager, IWorldService worldService)
        : base(position, healthValue, energyValue, EnvironmentType.Ground, timeManager)
    {
        _worldService = worldService;
        Color = Brushes.Red;
        ContactRadius = 0.005;
    }

    protected override void Die()
    {
        var waste = new OrganicWaste(Position, Energy);
        _worldService.AddEntity(waste);
        _worldService.RemoveEntity(this);
    }
}
