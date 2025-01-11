using System;

namespace Constants;

[Flags]
public enum CollisionMasks : uint
{
    None = 0,
    Walls = 1 << 1,       // Layer 2
    Character = 1 << 2,   // Layer 3
    // Add more layers as needed
    Enemy = 1 << 3,       // Layer 4
    Items = 1 << 4        // Layer 5
}

// Example of combining masks
public static class CollisionMaskExtensions
{
    public static bool HasMask(this CollisionMasks mask, CollisionMasks targetMask)
    {
        return (mask & targetMask) == targetMask;
    }
}