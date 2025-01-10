using System.Collections.Generic;
using Godot;

namespace Utilities;

public static class GridUtility
{
    public static List<Vector2I> GetSpreadPositions(List<Vector2I> gridCoordinates, Vector2I startPosition, int count)
    {
        // Ensure the count is not greater than the available positions
        count = Mathf.Min(count, gridCoordinates.Count);

        // Sort grid coordinates by distance from the start position
        gridCoordinates.Sort((a, b) 
            => a.DistanceTo(startPosition).CompareTo(b.DistanceTo(startPosition)));

        // Select the first 'count' closest coordinates
        List<Vector2I> result = new List<Vector2I>();
        for (int i = 0; i < count; i++)
        {
            result.Add(gridCoordinates[i]);
        }

        return result;
    }
}