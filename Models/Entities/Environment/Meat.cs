using System;
using Avalonia.Media;
using ecosystem.Models.Core;
using ecosystem.Services.Simulation;
using ecosystem.Services.World;
using ecosystem.Models.Animation;
using ecosystem.Helpers;

namespace ecosystem.Models.Entities.Environment;

public class Meat : LifeForm, IStaticSpriteEntity
{
    public ISprite? Sprite { get; private set; }
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

        try
        {
            Sprite = new StaticSprite(AssetHelper.GetAssetPath("Meat.png"));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load meat sprite: {ex.Message}");
        }
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
