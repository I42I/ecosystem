using ecosystem.Models.Behaviors.Base;
using ecosystem.Models.Entities.Animals;
using ecosystem.Services.World;
using ecosystem.Models.Entities.Environment;

namespace ecosystem.Models.Behaviors.Movement;

public class EnvironmentSeekingBehavior : IBehavior<Animal>
{
    private readonly IWorldService _worldService;

    public EnvironmentSeekingBehavior(IWorldService worldService)
    {
        _worldService = worldService;
    }
    public string Name => "EnvironmentSeeking";
    public int Priority => 3;

    public bool CanExecute(Animal animal)
    {
        var currentEnv = _worldService.GetEnvironmentAt(animal.Position);
        var bestEnv = animal.GetBestEnvironmentPreference(currentEnv);
        return bestEnv.Type == EnvironmentType.None;
    }

    public void Execute(Animal animal)
    {
        // Find better environment and move towards it
        // Simple implementation - move towards center if in wrong environment
        var centerX = _worldService.Grid.Width / 2;
        var centerY = _worldService.Grid.Height / 2;

        var dx = centerX - animal.Position.X;
        var dy = centerY - animal.Position.Y;

        var length = System.Math.Sqrt(dx * dx + dy * dy);
        if (length > 0)
        {
            dx /= length;
            dy /= length;
            animal.Move(dx, dy);
        }
    }
}