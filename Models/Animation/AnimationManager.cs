using System;
using System.Collections.Generic;

namespace ecosystem.Models.Animation;

public interface IAnimationManager
{
    void PlayAnimation(IAnimationEvent animEvent);
    void Update(double deltaTime);
    AnimationState CurrentState { get; }
    bool HasQueuedAnimations { get; }
}

public class AnimationManager : IAnimationManager
{
    private readonly AnimatedSprite _sprite;
    private readonly Queue<IAnimationEvent> _animationQueue = new();
    private IAnimationEvent? _currentAnimation;
    private double _idleStateTimer;
    private const double IDLE_STATE_DURATION = 2.0;

    public AnimationState CurrentState { get; private set; } = AnimationState.Idle;
    public bool HasQueuedAnimations => _animationQueue.Count > 0;

    public AnimationManager(AnimatedSprite sprite)
    {
        _sprite = sprite;
        _sprite.SetState(AnimationState.Idle);
    }

    public void PlayAnimation(IAnimationEvent animEvent)
    {
        if (animEvent.IsBlocking)
        {
            _currentAnimation = animEvent;
            CurrentState = animEvent.State;
            _sprite.SetState(CurrentState);
            _animationQueue.Clear();
        }
        else
        {
            _animationQueue.Enqueue(animEvent);
        }
    }

    public void Update(double deltaTime)
    {
        _sprite.Update(deltaTime);

        if (_currentAnimation?.IsBlocking == true)
        {
            if (_sprite.IsAnimationComplete)
            {
                _currentAnimation = null;
                if (_animationQueue.Count == 0)
                {
                    CurrentState = AnimationState.Idle;
                    _sprite.SetState(CurrentState);
                }
            }
            return;
        }

        if (_animationQueue.Count > 0)
        {
            _currentAnimation = _animationQueue.Dequeue();
            CurrentState = _currentAnimation.State;
            _sprite.SetState(CurrentState);
            return;
        }

        if (!_currentAnimation?.IsBlocking == true)
        {
            _idleStateTimer -= deltaTime;
            if (_idleStateTimer <= 0)
            {
                _idleStateTimer = IDLE_STATE_DURATION;
                CurrentState = CurrentState == AnimationState.Idle ? 
                    AnimationState.IdleAlt : AnimationState.Idle;
                _sprite.SetState(CurrentState);
            }
        }
    }
}