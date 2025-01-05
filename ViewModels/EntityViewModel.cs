using System;
using System.ComponentModel;
using Avalonia.Media;
using ecosystem.Models.Core;
using ecosystem.Models.Radius;
using ecosystem.Models.Animation;
using ecosystem.Models.Entities.Animals;
using Avalonia.Media.Imaging;
using Avalonia;
using ecosystem.Models.Behaviors.Movement;

namespace ecosystem.ViewModels;

public class EntityViewModel : ViewModelBase
{
    private readonly Entity _entity;
    private double _displayWidth = 800;
    private double _displayHeight = 600; 

    public Entity Entity => _entity;

    public EntityViewModel(Entity entity)
    {
        _entity = entity;
        _entity.PropertyChanged += Entity_PropertyChanged;
        _entity.Stats.PropertyChanged += Stats_PropertyChanged;
    }

    public double DisplayX => _entity.Position.X * _displayWidth;
    public double DisplayY => _entity.Position.Y * _displayHeight;
    public IBrush? Color => _entity.Color;
    public string StatsText
    {
        get
        {
            var stats = $"{_entity.Stats.DisplayStats}";
            var behavior = _entity.Stats.CurrentBehavior;
            return $"{stats}\n{behavior ?? "None"}";
        }
    }

    public double ScaledVisionRadius => Entity is IHasVisionRange e ? e.VisionRadius * _displayWidth : 0;
    public double ScaledContactRadius => Entity is IHasContactRange e ? e.ContactRadius * _displayWidth : 0;
    public double ScaledRootRadius => Entity is IHasRootSystem e ? e.RootRadius * _displayWidth : 0;
    public double CenteredX => DisplayX - ScaledVisionRadius / 2;
    public double CenteredY => DisplayY - ScaledVisionRadius / 2;
    
    public double CenteredContactX => DisplayX - ScaledContactRadius / 2;
    public double CenteredContactY => DisplayY - ScaledContactRadius / 2;
    
    public double CenteredRootX => DisplayX - ScaledRootRadius / 2;
    public double CenteredRootY => DisplayY - ScaledRootRadius / 2;

    public void UpdateDisplaySize(double width, double height)
    {
        _displayWidth = width;
        _displayHeight = height;
        OnPropertyChanged(nameof(DisplayX));
        OnPropertyChanged(nameof(DisplayY));
        OnPropertyChanged(nameof(ScaledVisionRadius));
        OnPropertyChanged(nameof(ScaledContactRadius));
        OnPropertyChanged(nameof(ScaledRootRadius));
        OnPropertyChanged(nameof(CenteredX));
        OnPropertyChanged(nameof(CenteredY));
        OnPropertyChanged(nameof(CenteredContactX));
        OnPropertyChanged(nameof(CenteredContactY));
        OnPropertyChanged(nameof(CenteredRootX));
        OnPropertyChanged(nameof(CenteredRootY));
        OnPropertyChanged(nameof(SpriteSize));
        OnPropertyChanged(nameof(SpriteCenteredX));
        OnPropertyChanged(nameof(SpriteCenteredY));
    }

    private void Entity_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Entity.Position))
        {
            OnPropertyChanged(nameof(DisplayX));
            OnPropertyChanged(nameof(DisplayY));
        }
        else if (e.PropertyName == nameof(Entity.Stats))
        {
            OnPropertyChanged(nameof(StatsText));
        }
    }

    private void Stats_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Entity.Stats.DisplayStats) ||
            e.PropertyName == nameof(Entity.Stats.CurrentBehavior))
        {
            OnPropertyChanged(nameof(StatsText));
        }
    }

    public bool HasSprite => _entity is IAnimatable || _entity is IStaticSpriteEntity;

    public double SpriteSize => Entity switch
    {
        IAnimatable => ScaledContactRadius * 3,
        IStaticSpriteEntity => ScaledContactRadius * 2,
        _ => ScaledContactRadius
    };

    public double SpriteCenteredX => DisplayX - SpriteSize / 2;
    public double SpriteCenteredY => DisplayY - SpriteSize / 2;
    
    public IImage? SpriteBitmap
    {
        get
        {
            if (_entity is IAnimatable animatable && animatable.Sprite != null)
            {
                var sourceRect = animatable.Sprite.GetSourceRect();
                return new CroppedBitmap(
                    animatable.Sprite.SpriteSheet,
                    new PixelRect(
                        (int)sourceRect.X, 
                        (int)sourceRect.Y,
                        (int)sourceRect.Width, 
                        (int)sourceRect.Height));
            }
            else if (_entity is IStaticSpriteEntity staticEntity && staticEntity.Sprite != null)
            {
                return staticEntity.Sprite.SpriteSheet;
            }
            return null;
        }
    }

    private double _lastDirection = 1;
    public double MovementDirection
    {
        get
        {
            if (_entity is MoveableEntity moveable)
            {
                var direction = moveable.CurrentDirectionX;
                if (Math.Abs(direction) > 0.01)
                {
                    _lastDirection = direction < 0 ? -1 : 1;
                }
                return _lastDirection;
            }
            return 1;
        }
    }

    public void UpdateAnimation(double deltaTime)
    {
        if (_entity is IAnimatable animatable)
        {
            animatable.UpdateAnimation(deltaTime);
            OnPropertyChanged(nameof(SpriteBitmap));
            OnPropertyChanged(nameof(MovementDirection));
        }
    }
}