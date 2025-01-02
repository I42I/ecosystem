namespace ecosystem.Models.Animation;

public interface IAnimatable
{
    AnimatedSprite? Sprite { get; }
    AnimationState CurrentState { get; }
    void UpdateAnimation(double deltaTime);
}