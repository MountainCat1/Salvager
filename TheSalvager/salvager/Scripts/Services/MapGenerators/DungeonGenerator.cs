using System;
using Godot;
using System.Collections.Generic;
using Services.Abstractions;

namespace Services.MapGenerators
{
    public partial class DungeonGenerator : Node2D, IMapGenerator
    {
        [Inject] private IRoomDecorator _roomDecorator = null!;
        
        public event Action? MapGenerated;
        public MapData? MapData { get; private set; }

        [Export] private int _seed = 696969;

        [Export] private Vector2I _gridSize = new Vector2I(50, 50);

        [Export] private int _roomCount = 10;
        [Export] private Vector2I _roomMinSize = new Vector2I(5, 5);
        [Export] private Vector2I _roomMaxSize = new Vector2I(10, 10);

        [Export] private TileMapLayer _wallTileMap = null!;
        [Export] private TileMapLayer _floorTileMap = null!;
        [Export] private TileMapLayer _shadowTileMap = null!;

        [Export] private int _floorTileId = (int)TileType.Floor;
        [Export] private int _wallTileId = (int)TileType.Wall;

        private Random _random = new Random();

        [Export] private bool _run = false;

        public override void _Ready()
        {
            _random = _seed == 0 ? new Random() : new Random(_seed);

            if (_run)
                GenerateDungeon();
        }

        private void GenerateDungeon()
        {
            var grid = new int[_gridSize.X, _gridSize.Y];
            var rooms = GenerateRooms(grid);

            ConnectRooms(grid, rooms);

            DrawDungeon(grid);

            GD.Print("===== Map generated =====");

            GD.Print("Decorating rooms...");
            _roomDecorator.DecorateRooms(rooms, tileSize: _wallTileMap.TileSet.TileSize.X);
            
            MapData = new MapData(_gridSize, grid, _wallTileMap.TileSet.TileSize, rooms);

            MapGenerated?.Invoke();
        }

        private List<RoomData> GenerateRooms(int[,] grid)
        {
            var rooms = new List<RoomData>();

            for (int i = 0; i < _roomCount; i++)
            {
                int roomWidth = _random.Next(_roomMinSize.X, _roomMaxSize.X + 1);
                int roomHeight = _random.Next(_roomMinSize.Y, _roomMaxSize.Y + 1);

                int x = _random.Next(1, _gridSize.X - roomWidth - 1);
                int y = _random.Next(1, _gridSize.Y - roomHeight - 1);

                var roomData = new RoomData { RoomID = i };

                for (int roomX = x; roomX < x + roomWidth; roomX++)
                {
                    for (int roomY = y; roomY < y + roomHeight; roomY++)
                    {
                        grid[roomX, roomY] = (int)TileType.Floor;
                        roomData.Positions.Add(new Vector2I(roomX, roomY));
                    }
                }

                rooms.Add(roomData);
                GD.Print($"Room {i} created at: {x}, {y}");
            }

            return rooms;
        }

        private void ConnectRooms(int[,] grid, List<RoomData> rooms)
        {
            for (int i = 0; i < rooms.Count - 1; i++)
            {
                var startRoom = rooms[i];
                var endRoom = rooms[i + 1];

                var start = startRoom.Positions[_random.Next(startRoom.Positions.Count)];
                var end = endRoom.Positions[_random.Next(endRoom.Positions.Count)];

                CreateCorridor(grid, start, end);

                startRoom.ConnectedRoomIDs.Add(endRoom.RoomID);
                endRoom.ConnectedRoomIDs.Add(startRoom.RoomID);

                GD.Print($"Corridor created between Room {startRoom.RoomID} and Room {endRoom.RoomID}");
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

                grid[current.X, current.Y] = (int)TileType.Floor;
            }

            GD.Print($"Corridor created from: {start} to {end}");
        }

        private void DrawDungeon(int[,] grid)
        {
            var wallTiles = new Godot.Collections.Array<Vector2I>();

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
                    }
                }
            }

            _wallTileMap.SetCellsTerrainConnect(wallTiles, 0, 0);

            GD.Print("Dungeon drawn on TileMap");
        }
    }
}
