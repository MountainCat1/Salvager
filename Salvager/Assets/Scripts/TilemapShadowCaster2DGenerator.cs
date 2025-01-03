using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

[RequireComponent(typeof(Tilemap))]
[RequireComponent(typeof(TilemapCollider2D))]
public class TilemapShadowCaster2DGenerator : MonoBehaviour
{
    [Tooltip("Parent object that will hold generated ShadowCaster2D children")]
    public Transform shadowCastersParent;

    private Tilemap tilemap;
    private TilemapCollider2D tilemapCollider;
    private CompositeCollider2D compositeCollider; // Optional if using Composite

    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        tilemapCollider = GetComponent<TilemapCollider2D>();
        compositeCollider = GetComponent<CompositeCollider2D>();
    }

    [ContextMenu("Generate Shadow Casters")]
    void GenerateShadowCasters()
    {
        // Clear previous shadow caster children
        if (shadowCastersParent == null) shadowCastersParent = this.transform;
        for (int i = shadowCastersParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(shadowCastersParent.GetChild(i).gameObject);
        }

        // Get the collider shape count from the composite, or from the tilemap collider
        int pathCount;
        if (compositeCollider != null)
        {
            pathCount = compositeCollider.pathCount;
        }
        else
        {
            // For non-composite colliders, you can iterate over each shape in the tilemapCollider
            // but typically it’s more complicated. Composite is recommended.
            // pathCount = tilemapCollider.GetShapesCount();
            throw new NotImplementedException("Non-composite collider not supported");
        }

        // Iterate over each shape and create a ShadowCaster2D
        for (int i = 0; i < pathCount; i++)
        {
            Vector2[] points;
            int pointCount;

            if (compositeCollider != null)
            {
                pointCount = compositeCollider.GetPathPointCount(i);
                points = new Vector2[pointCount];
                compositeCollider.GetPath(i, points);
            }
            else
            {
                // Fallback if you’re not using CompositeCollider2D
                // tilemapCollider.GetShape(i, shape);
                // Convert shape info to points[]...
                continue;
            }

            // Create a new GameObject for the shadow caster
            GameObject shadowCasterObj = new GameObject("ShadowCaster2D_" + i);
            shadowCasterObj.transform.SetParent(shadowCastersParent, false);

            // Add the ShadowCaster2D component
            ShadowCaster2D shadowCaster = shadowCasterObj.AddComponent<ShadowCaster2D>();

            // Convert the Vector2[] points into the Vector3[] required by ShadowCaster2D
            Vector3[] shadowPoints = System.Array.ConvertAll(points, p => (Vector3)p);

            // Assign shape to the ShadowCaster2D
            // Depending on your version of Unity, you might need to use a serialized “mesh” approach.
            // In modern versions you can do:
            shadowCaster.shapePath = shadowPoints;
            shadowCaster.useRendererSilhouette = false;
            shadowCaster.selfShadows = false;

            // You might need to flip the shape if it’s winding incorrectly
            // or manually check for clockwise vs. counter-clockwise winding.
        }
    }
}
