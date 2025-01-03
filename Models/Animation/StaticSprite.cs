using Avalonia.Media.Imaging;
using Avalonia;

namespace ecosystem.Models.Animation;

public class StaticSprite : ISprite
{
    private readonly Bitmap _image;

    public StaticSprite(string imagePath)
    {
        _image = new Bitmap(imagePath);
    }

    public Bitmap SpriteSheet => _image;

    public Rect GetSourceRect()
    {
        return new Rect(0, 0, _image.Size.Width, _image.Size.Height);
    }
}