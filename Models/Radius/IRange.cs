namespace ecosystem.Models.Radius;

public interface IHasContactRange
{
    double ContactRadius { get; }
}

public interface IHasVisionRange
{
    double VisionRadius { get; }
}

public interface IHasRootSystem
{
    double RootRadius { get; }
    double SeedRadius { get; }
}