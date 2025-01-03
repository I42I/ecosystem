namespace ecosystem.Models.Animation;

public interface ISprite
{
    Avalonia.Media.Imaging.Bitmap SpriteSheet { get; }
    Avalonia.Rect GetSourceRect();
}

public interface IStaticSpriteEntity
{
    ISprite? Sprite { get; }
}