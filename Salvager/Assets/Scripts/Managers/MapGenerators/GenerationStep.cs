using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Services.MapGenerators
{
    [Serializable]
    public class GenerateMapSettings
    {
        [SerializeField] public int roomCount = 5;
        [SerializeField] public Vector2Int roomMinSize = default;
        [SerializeField] public Vector2Int roomMaxSize = default;
        [SerializeField] public Vector2Int gridSize = default;
        [SerializeField] public float tileSize = 1f;
        [SerializeField] public int seed;

        public static GenerateMapSettings GenerateRandom()
        {
            return new GenerateMapSettings
            {
                roomCount = UnityEngine.Random.Range(10, 20),
                roomMaxSize = new Vector2Int(UnityEngine.Random.Range(5, 10), UnityEngine.Random.Range(5, 10)),
                roomMinSize = new Vector2Int(UnityEngine.Random.Range(3, 4), UnityEngine.Random.Range(3, 4)),
                gridSize = new Vector2Int(50, 50),
                seed = UnityEngine.Random.Range(0, 1000000),
                tileSize = 1f
            };
        }
    }
    
    public enum MapLayer
    {
        Wall,
        Floor,
        Shadow
    }

    public class GenerateMapData
    {
        public int[,] Grid { get; set; }
        public Vector2Int GridSize { get; set; }
        public List<RoomData> Rooms { get; set; }

        public GenerateMapData(GenerateMapSettings settings)
        {
            GridSize = settings.gridSize;
            Grid = new int[GridSize.x, GridSize.y];
            Rooms = new List<RoomData>();
        }

        public void SetTile(int x, int y, TileType value)
        {
            Grid[x, y] = (int)value;
        }

        public int GetTile(int x, int y)
        {
            return Grid[x, y];
        }

        public ICollection<Vector2Int> CreateTileList(TileType floor)
        {
            var tiles = new List<Vector2Int>();

            for (int x = 0; x < GridSize.x; x++)
            {
                for (int y = 0; y < GridSize.y; y++)
                {
                    if (GetTile(x, y) == (int)floor)
                    {
                        tiles.Add(new Vector2Int(x, y));
                    }
                }
            }

            return tiles;
        }

        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < GridSize.x && y >= 0 && y < GridSize.y;
        }
    }

    public abstract class GenerationStep : MonoBehaviour
    {
        public abstract void Generate(GenerateMapData data, GenerateMapSettings settings, Random random);
    }
}