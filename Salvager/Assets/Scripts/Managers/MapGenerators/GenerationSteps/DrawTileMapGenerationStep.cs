using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

namespace Services.MapGenerators.GenerationSteps
{
    public class DrawTileMapGenerationStep : GenerationStep
    {
        [SerializeField] private Tilemap wallTileMap = null!;
        [SerializeField] private Tilemap floorTileMap = null!;
        
        [SerializeField] private TileBase wallTile = null!;
        [SerializeField] private TileBase floorTile = null!;
        
        public override void Generate(GenerateMapData data, GenerateMapSettings settings, Random random)
        {
            DrawTiles(data, TileType.Floor, floorTileMap, floorTile);
            DrawTiles(data, TileType.Wall, wallTileMap, wallTile);
        }

        private void DrawTiles(GenerateMapData data, TileType tileType, Tilemap tileMap, TileBase tileBase)
        {
            var tilePositions = data.CreateTileList(tileType).Select(v => new Vector3Int(v.x ,v.y, 0)).ToArray();
            var tiles = data.CreateTileList(tileType).Select(x => tileBase).ToArray();
            tileMap.SetTiles(tilePositions, tiles);
        }
    }
}