using System;
using System.Collections.Generic;
using System.Linq;
using ShadowModule;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using Zenject;


public partial class TilemapShadowGenerator : MonoBehaviour
{
    [Inject] private IMapGenerator _mapGenerator;
    
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private ShadowCaster2D shadowCasterPrefab;

    [SerializeField] private float margin = 0.1f;

    // Called when the node enters the scene tree for the first time.
    
    private float _lastMargin = -1;

    private void Awake()
    {
        _mapGenerator.MapGenerated += OnMapGenerated;
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
        
        var getAllTilePositionsOfType = GetAllTilePositions(tilemap).Select(x => new Vector2Int(x.x, x.y)).ToList();

        var clusters = TileCluster.GetConnectedClusters(getAllTilePositionsOfType);

        foreach (var cluster in clusters)
        {
            var wrapPolygon = TilePolygon.GetWrappingPolygon(cluster, tilemap.cellSize.x);

            var shadowCaster = Instantiate(this.shadowCasterPrefab, transform);
            shadowCaster.transform.SetParent(transform);
            
            wrapPolygon = PolygonMargin.ApplyMargin(wrapPolygon, margin);

            ShadowCasterUtility.UpdateShadowCasterShape(shadowCaster.gameObject, wrapPolygon.ToArray());
            
            // If the polygon is the longest we assume it's the outer wall, so we set it to not cast self shadow
            if(cluster.Count == clusters.Max(x => x.Count))
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