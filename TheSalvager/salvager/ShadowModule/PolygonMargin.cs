using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class PolygonMargin
{
    public static List<Vector2> ApplyMargin(List<Vector2> polygon, float padding)
    {
        if (polygon == null || polygon.Count < 3)
            throw new ArgumentException("Polygon must have at least three vertices.");

        // Calculate the centroid of the polygon
        var centroid = CalculateCentroid(polygon);

        // Apply padding by moving each vertex closer to or farther from the centroid
        var adjustedPolygon = polygon
            .Select(vertex =>
            {
                var direction = (vertex - centroid).Normalized();
                return vertex - direction * padding;
            })
            .ToList();

        return adjustedPolygon;
    }

    private static Vector2 CalculateCentroid(List<Vector2> polygon)
    {
        float xSum = 0, ySum = 0;

        foreach (var vertex in polygon)
        {
            xSum += vertex.X;
            ySum += vertex.Y;
        }

        return new Vector2(xSum / polygon.Count, ySum / polygon.Count);
    }
}
