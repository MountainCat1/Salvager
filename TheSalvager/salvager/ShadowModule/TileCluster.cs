using Godot;

namespace ShadowModule;

using System;
using System.Collections.Generic;
using System.Linq;


public class TileCluster
{
    public static List<List<Vector2>> GetConnectedClusters(List<Vector2> tilePositions)
    {
        var remainingTiles = new HashSet<Vector2>(tilePositions);
        var clusters = new List<List<Vector2>>();

        // Define the four cardinal directions for adjacency (side-touching)
        var directions = new List<Vector2>
        {
            new Vector2(0, 1),  // Up
            new Vector2(1, 0),  // Right
            new Vector2(0, -1), // Down
            new Vector2(-1, 0)  // Left
        };

        while (remainingTiles.Count > 0)
        {
            var cluster = new List<Vector2>();
            var queue = new Queue<Vector2>();

            // Start a new cluster from any remaining tile
            var startTile = remainingTiles.First();
            queue.Enqueue(startTile);
            remainingTiles.Remove(startTile);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                cluster.Add(current);

                // Check all neighboring tiles
                foreach (var direction in directions)
                {
                    var neighbor = current + direction;
                    if (remainingTiles.Contains(neighbor))
                    {
                        queue.Enqueue(neighbor);
                        remainingTiles.Remove(neighbor);
                    }
                }
            }

            clusters.Add(cluster);
        }

        return clusters;
    }
}
