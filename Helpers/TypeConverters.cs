using Avalonia.Data.Converters;
using ecosystem.Models.Entities.Animals;
using ecosystem.Models.Entities.Plants;
using ecosystem.Models.Core;

namespace ecosystem.Helpers;

public static class TypeConverters
{
    public static readonly IValueConverter IsAnimal = new FuncValueConverter<Entity, bool>(
        value => value is Animal
    );

    public static readonly IValueConverter IsPlant = new FuncValueConverter<Entity, bool>(
        value => value is Plant
    );
}