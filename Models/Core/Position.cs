namespace ecosystem.Models.Core;

public class Position
{
    public double X { get; set; }
    public double Y { get; set; }

    public Position(double x, double y)
    {
        X = x;
        Y = y;
    }

    public void Deconstruct(out double x, out double y)
    {
        x = X;
        y = Y;
    }

    public static implicit operator (double X, double Y)(Position position) => (position.X, position.Y);
    public static implicit operator Position((double X, double Y) tuple) => new(tuple.X, tuple.Y);
}
