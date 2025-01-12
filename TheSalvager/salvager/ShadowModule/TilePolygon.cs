using Godot;

namespace ShadowModule;

using System.Collections.Generic;
using System.Linq;

public static class TilePolygon
{
    public static List<Vector2> GetWrappingPolygon(List<Vector2> tilePositions, float tileSize)
    {
        // Create a set of edges
        var edges = new HashSet<(Vector2 Start, Vector2 End)>();

        foreach (var tile in tilePositions)
        {
            Vector2 bottomLeft = tile * tileSize;
            Vector2 bottomRight = bottomLeft + new Vector2(tileSize, 0);
            Vector2 topLeft = bottomLeft + new Vector2(0, tileSize);
            Vector2 topRight = bottomLeft + new Vector2(tileSize, tileSize);

            // Define edges for this tile
            var tileEdges = new List<(Vector2, Vector2)>
            {
                (bottomLeft, bottomRight),
                (bottomRight, topRight),
                (topRight, topLeft),
                (topLeft, bottomLeft)
            };

            foreach (var edge in tileEdges)
            {
                // Check if the reverse edge exists in the set
                if (!edges.Remove((edge.Item2, edge.Item1)))
                {
                    // Add the edge if the reverse edge doesn't exist
                    edges.Add(edge);
                }
            }
        }

        // The remaining edges form the outline
        return OrderEdges(edges);
    }

    private static List<Vector2> OrderEdges(HashSet<(Vector2 Start, Vector2 End)> edges)
    {
        // Order edges into a continuous path
        var orderedPolygon = new List<Vector2>();
        var edgeDict = edges.ToDictionary(e => e.Start, e => e.End);

        // Start with any edge
        var current = edgeDict.Keys.First();
        orderedPolygon.Add(current);

        while (edgeDict.TryGetValue(current, out var next))
        {
            orderedPolygon.Add(next);
            edgeDict.Remove(current);
            current = next;
        }

        return orderedPolygon;
    }
}
