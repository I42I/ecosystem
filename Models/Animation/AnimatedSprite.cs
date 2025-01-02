using System;
using Avalonia.Media.Imaging;
using System.Collections.Generic;
using Avalonia;

namespace ecosystem.Models.Animation;

public class AnimatedSprite
{
    private readonly Bitmap _spriteSheet;
    private readonly Dictionary<AnimationState, AnimationConfig> _animations;
    private double _animationTimer;
    private int _currentFrame;
    private AnimationState _currentState;

    public class AnimationConfig
    {
        public int StartFrame { get; }
        public int Row { get; }
        public int FrameCount { get; }
        public double FrameDuration { get; }
        public bool Loop { get; }

        public AnimationConfig(int row, int startFrame, int frameCount, double frameDuration, bool loop = true)
        {
            Row = row;
            StartFrame = startFrame;
            FrameCount = frameCount;
            FrameDuration = frameDuration;
            Loop = loop;
        }
    }

    public AnimatedSprite(string imagePath, int frameWidth, int frameHeight)
    {
        _spriteSheet = new Bitmap(imagePath);
        FrameWidth = frameWidth;
        FrameHeight = frameHeight;
        _animations = new Dictionary<AnimationState, AnimationConfig>();
    }

    public int FrameWidth { get; }
    public int FrameHeight { get; }
    public Bitmap SpriteSheet => _spriteSheet;

    public void AddAnimation(AnimationState state, AnimationConfig config)
    {
        _animations[state] = config;
    }

    public void SetState(AnimationState newState)
    {
        if (_currentState != newState && _animations.ContainsKey(newState))
        {
            _currentState = newState;
            _currentFrame = _animations[newState].StartFrame;
            _animationTimer = 0;
        }
    }

    public void Update(double deltaTime)
    {
        if (!_animations.ContainsKey(_currentState)) 
        {
            Console.WriteLine($"No animation found for state: {_currentState}");
            return;
        }

        var config = _animations[_currentState];
        _animationTimer += deltaTime;

        if (_animationTimer >= config.FrameDuration)
        {
            var oldFrame = _currentFrame;
            _animationTimer = 0;

            if (_currentFrame >= config.StartFrame + config.FrameCount - 1)
            {
                if (config.Loop)
                    _currentFrame = config.StartFrame;
                else
                    _currentFrame = config.StartFrame + config.FrameCount - 1;
            }
            else
            {
                _currentFrame++;
            }

            Console.WriteLine($"Frame changed: {oldFrame} -> {_currentFrame} " +
                            $"(State={_currentState}, Row={config.Row})");
        }
    }

    public Rect GetSourceRect()
    {
        var config = _animations[_currentState];
        var rect = new Rect(
            _currentFrame * FrameWidth,
            config.Row * FrameHeight,
            FrameWidth,
            FrameHeight);

        Console.WriteLine($"Source Rect: X={rect.X}, Y={rect.Y}, " +
                        $"W={rect.Width}, H={rect.Height}");
                        
        return rect;
    }
}