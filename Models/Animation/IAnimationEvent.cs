namespace ecosystem.Models.Animation;

public interface IAnimationEvent
{
    AnimationState State { get; }
    bool IsBlocking { get; }
}

public record AnimationEvent(AnimationState State, bool IsBlocking = false) : IAnimationEvent;