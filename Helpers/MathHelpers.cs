using System;
using ecosystem.Models.Core;
using ecosystem.Models.Entities.Animals;

namespace ecosystem.Helpers;

public static class MathHelper
{
    public static double CalculateDistance(Position pos1, Position pos2)
    {
        var dx = pos1.X - pos2.X;
        var dy = pos1.Y - pos2.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    public static bool IsInContactWith(this Animal animal, Animal other)
    {
        return CalculateDistance(animal.Position, other.Position) <= animal.ContactRadius;
    }
}