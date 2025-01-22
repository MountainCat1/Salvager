using System;
using System.Collections.Generic;
using System.Linq;
using Services.MapGenerators;
using ShadowModule;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Zenject;


public partial class TilemapShadowGenerator : MonoBehaviour
{
    [Inject] private IMapGenerator _mapGenerator;

    [FormerlySerializedAs("tilemap")] [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private ShadowCaster2D shadowCasterPrefab;

    [SerializeField] private float margin = 0.1f;

    [SerializeField] private bool debugClusters = false;

    // Called when the node enters the scene tree for the first time.

    private float _lastMargin = -1;

    private void Awake()
    {
        _mapGenerator.MapGenerated += OnMapGenerated;
    }

    private void OnDrawGizmos()
    {
        if (debugClusters)
        {
            var getAllTilePositionsOfType = GetAllTilePositions(wallTilemap).Select(x => new Vector2Int(x.x, x.y)).ToList();

            var clusters = TileCluster.GetConnectedClusters(getAllTilePositionsOfType);

            // List of predefined colors, or you can dynamically create random colors.
            List<Color> clusterColors = new List<Color>
            {
                Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta, Color.white, Color.black, Color.gray
            };

            int colorIndex = 0;

            foreach (var cluster in clusters)
            {
                var wrapPolygon = TilePolygon.GetWrappingPolygon(cluster, wallTilemap.cellSize.x);
                wrapPolygon = PolygonMargin.ApplyMargin(wrapPolygon, margin);

                // Cycle through colors or generate new ones.
                Gizmos.color = clusterColors[colorIndex % clusterColors.Count];
                colorIndex++;

                for (int i = 0; i < wrapPolygon.Count; i++)
                {
                    var next = wrapPolygon[(i + 1) % wrapPolygon.Count];
                    Gizmos.DrawLine(wrapPolygon[i], next);
                }
            }
        }
    }


    private void OnMapGenerated()
    {
        UpdateShadows();
    }

    public void UpdateShadows()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        var wallTiles = GetAllTilePositions(wallTilemap).Select(x => new Vector2Int(x.x, x.y)).ToList();
        var wallClusters = TileCluster.GetConnectedClusters(wallTiles);

        foreach (var cluster in wallClusters)
        {
            // If the polygon is the longest we assume it's the outer wall, so we ignore it
            if (cluster.Count == wallClusters.Max(x => x.Count))
                continue;
            
            var wrapPolygon = TilePolygon.GetWrappingPolygon(cluster, wallTilemap.cellSize.x);

            var shadowCaster = Instantiate(this.shadowCasterPrefab, transform);
            shadowCaster.transform.SetParent(transform);
            shadowCaster.name = "Wall Shadow Caster";

            wrapPolygon = PolygonMargin.ApplyMargin(wrapPolygon, margin);

            ShadowCasterUtility.UpdateShadowCasterShape(shadowCaster.gameObject, wrapPolygon.ToArray());
            
        }
        
        var floorTiles = GetAllTilePositions(floorTilemap).Select(x => new Vector2Int(x.x, x.y)).ToList();
        var floorClusters = TileCluster.GetConnectedClusters(floorTiles);

        foreach (var cluster in floorClusters)
        {
            var wrapPolygon = TilePolygon.GetWrappingPolygon(cluster, wallTilemap.cellSize.x);

            var shadowCaster = Instantiate(this.shadowCasterPrefab, transform);
            shadowCaster.transform.SetParent(transform);
            shadowCaster.name = "Floor Shadow Caster";

            wrapPolygon = PolygonMargin.ApplyMargin(wrapPolygon, -margin);

            ShadowCasterUtility.UpdateShadowCasterShape(shadowCaster.gameObject, wrapPolygon.ToArray());

            shadowCaster.selfShadows = false;
        }
    }


    /// <summary>
    /// Returns a list of all cell positions within the given Tilemap where a tile exists.
    /// </summary>
    public List<Vector3Int> GetAllTilePositions(Tilemap tm)
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        if (tm == null) return positions;

        // Get the bounding area (min and max inclusive) of all the cells in the tilemap
        BoundsInt bounds = tm.cellBounds;

        // Loop through each cell in the bounding box
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                for (int z = bounds.zMin; z < bounds.zMax; z++)
                {
                    Vector3Int cellPos = new Vector3Int(x, y, z);

                    // Check if there is actually a tile in this position
                    TileBase tile = tm.GetTile(cellPos);
                    if (tile != null)
                    {
                        positions.Add(cellPos);
                    }
                }
            }
        }

        return positions;
    }

    //
    // private void OnMapGenerated()
    // {
    // 	return; // TODO: remove this line
    // 	
    // 	var mapData = _dungeonGenerator.MapData!;
    // 	var walls = mapData
    // 		.GetAllTilePositionsOfType(TileType.Wall)
    // 		.Select(x => new Vector2(x.X, x.Y))
    // 		.ToList();
    //
    // 	var clusters = TileCluster.GetConnectedClusters(walls);
    //
    // 	foreach (var cluster in clusters)
    // 	{
    // 		var wrapPolygon = TilePolygon.GetWrappingPolygon(cluster, mapData.TileSize);
    // 		var shadowCaster = new LightOccluder2D();
    // 		AddChild(shadowCaster);
    //
    // 		var polygon = new OccluderPolygon2D();
    // 		polygon.SetPolygon(wrapPolygon.ToArray());
    // 		polygon.CullMode = OccluderPolygon2D.CullModeEnum.Clockwise;
    // 		
    // 		shadowCaster.SetOccluderPolygon(polygon);
    // 	}
    // }
}