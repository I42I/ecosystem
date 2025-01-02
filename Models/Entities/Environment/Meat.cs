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
    protected bool _hasCreatedWaste = false;

    public Meat(Position position, int healthValue, int energyValue, ITimeManager timeManager, IWorldService worldService)
        : base(position, healthValue, energyValue, EnvironmentType.Ground, timeManager)
    {
        _worldService = worldService;
        Color = Brushes.Red;
        ContactRadius = 0.007;
    }

    protected override void Die()
    {
        if (_hasCreatedWaste) return;
        _hasCreatedWaste = true;

        var waste = new OrganicWaste(Position, Energy, _worldService);
        _worldService.AddEntity(waste);
        _worldService.RemoveEntity(this);
    }
}
