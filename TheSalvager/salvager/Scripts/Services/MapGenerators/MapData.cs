using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Services.Abstractions;

namespace Services.MapGenerators;

public class MapData
{
    private readonly Dictionary<Vector2I, TileType> _mapData = new();
    private readonly Dictionary<TileType, ICollection<Vector2>> _tilePositions = new();
    private Vector2 _tileSize;

    public MapData(Vector2I gridSize, int[,] grid, Vector2 tileSize)
    {
        for (int x = 0; x < gridSize.X; x++)
        {
            for (int y = 0; y < gridSize.Y; y++)
            {
                var tileType = (TileType)grid[x, y];
                
                _mapData[new Vector2I(x, y)] = tileType;
                
                if (!_tilePositions.ContainsKey(tileType))
                {
                    _tilePositions[tileType] = new List<Vector2>();
                }
                _tilePositions[tileType].Add(new Vector2(x, y) * tileSize);
            }
        }
        _tileSize = tileSize;
    }
    
    public TileType GetTileType(Vector2I position)
    {
        return _mapData[position];
    }
    
    public Vector2 GetRandomPositionTileOfType(TileType tileType)
    {
        var tiles = new List<Vector2>();

        foreach (var (position, type) in _mapData)
        {
            if (type == tileType)
            {
                tiles.Add(position);
            }
        }

        return tiles[new Random().Next(0, tiles.Count)] * _tileSize;
    }
    
    public List<Vector2> GetSpreadPositions(Vector2 startPosition, int count, TileType tileType)
    {
        // Collect all tiles of the specified type
        var tiles = _tilePositions[tileType].ToList();

        // Sort the tiles by distance to the start position
        tiles.Sort((a, b) => 
            a.DistanceSquaredTo(startPosition).CompareTo(b.DistanceSquaredTo(startPosition)));

        // Return up to the requested count of tiles
        count = Math.Min(count, tiles.Count);
        return tiles.GetRange(0, count);
    }
}