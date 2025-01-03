using Godot;
using System;

public partial class DungeonGenerator : Node2D
{
    [Export] private Vector2I _gridSize = new Vector2I(50, 50); // Size of the dungeon grid
    [Export] private int _roomCount = 10; // Number of rooms
    [Export] private Vector2I _roomMinSize = new Vector2I(5, 5); // Minimum room size
    [Export] private Vector2I _roomMaxSize = new Vector2I(10, 10); // Maximum room size

    [Export] private TileMapLayer _wallTileMap = null!; // TileMap to draw the dungeon
    [Export] private TileMapLayer _floorTileMap = null!; // TileMap to draw the dungeon
    [Export] private int _floorTileId = 0; // Tile ID for floor
    [Export] private int _wallTileId = 1; // Tile ID for wall

    private Random _random = new Random();

    [Export] private bool _run = false;

    public override void _Ready()
    {
        // log tileset
        var tileSet = _wallTileMap.TileSet;
        var tileCount = tileSet.GetSource(0).GetTilesCount();
        for (int i = 0; i < tileCount; i++)
        {
            GD.Print("Tile " + i);
        }

        if (_run)
            GenerateDungeon();
        
        var tileSet2 = _floorTileMap.TileSet;
        var tileCount2 = tileSet2.GetSource(0).GetTilesCount();
        for (int i = 0; i < tileCount2; i++)
        {
            GD.Print("Tile " + i);
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
                grid[i, j] = 1; // Mark floor
            }
        }

        GD.Print("Room created at: " + x + ", " + y);
    }

    private void ConnectRooms(int[,] grid)
    {
        // Simple way to connect rooms: create corridors between random floor tiles
        var floorTiles = new System.Collections.Generic.List<Vector2I>();
        for (int x = 0; x < _gridSize.X; x++)
        {
            for (int y = 0; y < _gridSize.Y; y++)
            {
                if (grid[x, y] == 1)
                {
                    floorTiles.Add(new Vector2I(x, y));
                }
            }

            GD.Print("Floor tiles: " + floorTiles.Count);
        }

        for (int i = 0; i < _roomCount - 1; i++)
        {
            var start = floorTiles[_random.Next(floorTiles.Count)];
            var end = floorTiles[_random.Next(floorTiles.Count)];

            CreateCorridor(grid, start, end);

            GD.Print("Corridor created between: " + start + " and " + end);
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

            grid[current.X, current.Y] = 1; // Mark floor
        }

        GD.Print("Corridor created from: " + start + " to " + end);
    }

    private void DrawDungeon(int[,] grid)
    {
        // _tileMap.Clear();

        for (int x = 0; x < _gridSize.X; x++)
        {
            for (int y = 0; y < _gridSize.Y; y++)
            {
                var tileId = grid[x, y] == 1 ? _floorTileId : _wallTileId;
                if (tileId == _floorTileId)
                {
                    SetFloor(x, y);
                }
                else if(tileId == _wallTileId)
                {
                    SetWall(x, y);
                }
            }
        }
        
        GD.Print("Dungeon drawn on TileMap");
    }
    
    private void SetWall(int x, int y)
    {
        _wallTileMap.SetCell(new Vector2I(x, y), 0, new Vector2I(0, 0));
    }
    
    private void SetFloor(int x, int y)
    {
        _floorTileMap.SetCell(new Vector2I(x, y), 0, new Vector2I(0, 1));
    }
}