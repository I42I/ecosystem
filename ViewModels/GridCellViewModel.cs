using Avalonia.Media;

namespace ecosystem.ViewModels;

public class GridCellViewModel : ViewModelBase
{
    public double X { get; }
    public double Y { get; }
    public double Width { get; }
    public double Height { get; }
    public IBrush Color { get; }

    public GridCellViewModel(double x, double y, double width, double height, IBrush color)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        Color = color;
    }
}