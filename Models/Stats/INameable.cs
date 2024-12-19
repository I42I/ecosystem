namespace ecosystem.Models.Stats;

public interface INameable
{
    string DisplayName { get; }
    int TypeId { get; }
}