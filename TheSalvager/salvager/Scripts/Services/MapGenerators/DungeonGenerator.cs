using System;
using Godot;
using Godot.Collections;
using Services.Abstractions;

namespace Services.MapGenerators;

public partial class DungeonGenerator : Node2D, IMapGenerator
{
    public event Action? MapGenerated;
    public MapData? MapData { get; private set; }
    
    [Export] private int _seed = 696969;

    // Size of the dungeon grid
    [Export] private Vector2I _gridSize = new Vector2I(50, 50);

    // Room generation parameters
    [Export] private int _roomCount = 10;
    [Export] private Vector2I _roomMinSize = new Vector2I(5, 5);
    [Export] private Vector2I _roomMaxSize = new Vector2I(10, 10);

    // TileMap layers
    [Export] private TileMapLayer _wallTileMap = null!;
    [Export] private TileMapLayer _floorTileMap = null!;
    [Export] private TileMapLayer _shadowTileMap = null!;

    [Export] private int _floorTileId = (int)TileType.Floor;
    [Export] private int _wallTileId = (int)TileType.Wall;

    private Random _random = new Random();

    [Export] private bool _run = false;

    public override void _Ready()
    {
        _random = new Random(_seed);
        
        LogTileSetInfo(_wallTileMap, "WALL");
        LogTileSetInfo(_floorTileMap, "FLOOR");
        LogTileSetInfo(_shadowTileMap, "SHADOW");

        if (_run)
            GenerateDungeon();
    }

    private void LogTileSetInfo(TileMapLayer tileMap, string label)
    {
        var tileSet = tileMap.TileSet;
        var tileCount = tileSet.GetSource(0).GetTilesCount();
        GD.Print($"{label} - Tile count: {tileCount}");
        for (int i = 0; i < tileCount; i++)
        {
            GD.Print($"Tile {i}");
        }
    }

    private void GenerateDungeon()
    {
        var grid = new int[_gridSize.X, _gridSize.Y];

        // Create rooms
        for (int i = 0; i < _roomCount; i++)
        {
            CreateRoom(grid);
        }

        // Create corridors between rooms
        ConnectRooms(grid);

        // Draw the dungeon on the TileMap
        DrawDungeon(grid);
        
        GD.Print("===== Map generated =====");
        
        MapData = new MapData(_gridSize, grid, tileSize: _wallTileMap.TileSet.TileSize);
        
        MapGenerated?.Invoke();
    }

    private void CreateRoom(int[,] grid)
    {
        int roomWidth = _random.Next(_roomMinSize.X, _roomMaxSize.X + 1);
        int roomHeight = _random.Next(_roomMinSize.Y, _roomMaxSize.Y + 1);

        int x = _random.Next(1, _gridSize.X - roomWidth - 1);
        int y = _random.Next(1, _gridSize.Y - roomHeight - 1);

        for (int i = x; i < x + roomWidth; i++)
        {
            for (int j = y; j < y + roomHeight; j++)
            {
                grid[i, j] = (int)TileType.Floor; // Mark floor
            }
        }

        GD.Print($"Room created at: {x}, {y}");
    }

    private void ConnectRooms(int[,] grid)
    {
        var floorTiles = new System.Collections.Generic.List<Vector2I>();

        for (int x = 0; x < _gridSize.X; x++)
        {
            for (int y = 0; y < _gridSize.Y; y++)
            {
                if (grid[x, y] == (int)TileType.Floor)
                {
                    floorTiles.Add(new Vector2I(x, y));
                }
            }
        }

        GD.Print($"Floor tiles: {floorTiles.Count}");

        for (int i = 0; i < _roomCount - 1; i++)
        {
            var start = floorTiles[_random.Next(floorTiles.Count)];
            var end = floorTiles[_random.Next(floorTiles.Count)];

            CreateCorridor(grid, start, end);

            GD.Print($"Corridor created between: {start} and {end}");
        }
    }

    private void CreateCorridor(int[,] grid, Vector2I start, Vector2I end)
    {
        var current = start;

        while (current != end)
        {
            if (current.X != end.X)
            {
                current.X += Math.Sign(end.X - current.X);
            }
            else if (current.Y != end.Y)
            {
                current.Y += Math.Sign(end.Y - current.Y);
            }

            grid[current.X, current.Y] = (int)TileType.Floor; // Mark floor
        }

        GD.Print($"Corridor created from: {start} to {end}");
    }

    private void DrawDungeon(int[,] grid)
    {
        var wallTiles = new Array<Vector2I>();

        for (int x = 0; x < _gridSize.X; x++)
        {
            for (int y = 0; y < _gridSize.Y; y++)
            {
                if (grid[x, y] == (int)TileType.Floor)
                {
                    _floorTileMap.SetCell(new Vector2I(x, y), 0, new Vector2I(_random.Next(0, 4), _random.Next(0, 4)));
                }
                else
                {
                    wallTiles.Add(new Vector2I(x, y));
                    _shadowTileMap.SetCell(new Vector2I(x, y + 1), 0, new Vector2I(0, 0));
                    _shadowTileMap.SetCell(new Vector2I(x, y), 0, new Vector2I(1, 0));
                }
            }
        }

        _wallTileMap.SetCellsTerrainConnect(wallTiles, 0, 0);

        GD.Print("Dungeon drawn on TileMap");
    }
}
