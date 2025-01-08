using System;
using System.Collections.Generic;
using Godot;
using Services.Abstractions;

namespace Services.MapGenerators;

public class MapData
{
    private readonly Dictionary<Vector2, TileType> _mapData = new();
    private Vector2 _tileSize;

    public MapData(Vector2I gridSize, int[,] grid, Vector2 tileSize)
    {
        for (int x = 0; x < gridSize.X; x++)
        {
            for (int y = 0; y < gridSize.Y; y++)
            {
                var tileType = (TileType)grid[x, y];
                
                _mapData[new Vector2(x, y)] = tileType;
            }
        }
        _tileSize = tileSize;
    }
    
    public TileType GetTileType(Vector2 position)
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
}