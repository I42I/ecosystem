namespace ecosystem.Models.Entities.Environment;

public abstract class EnvironmentBase
{
    public string Name { get; set; }
    public (double Width, double Height) Size { get; set; }

    protected EnvironmentBase(string name, (double Width, double Height) size)
    {
        Name = name;
        Size = size;
    }
}