using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Services.Abstractions;

namespace Services.MapGenerators
{
    public class MapData
    {
        public float TileSize => _tileSize.X;

        
        private readonly Dictionary<Vector2I, TileType> _mapData = new();
        private readonly Dictionary<TileType, ICollection<Vector2>> _tilePositions = new();
        private readonly Dictionary<int, RoomData> _rooms = new(); // Stores room data
        private Vector2 _tileSize;

        public MapData(Vector2I gridSize, int[,] grid, Vector2 tileSize, List<RoomData> rooms)
        {
            // Populate tile data
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

            // Populate room data
            foreach (var room in rooms)
            {
                _rooms[room.RoomID] = room;
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
            var tiles = _tilePositions[tileType].ToList();

            tiles.Sort((a, b) => 
                a.DistanceSquaredTo(startPosition).CompareTo(b.DistanceSquaredTo(startPosition)));

            count = Math.Min(count, tiles.Count);
            return tiles.GetRange(0, count);
        }

        public RoomData? GetRoomData(int roomID)
        {
            return _rooms.ContainsKey(roomID) ? _rooms[roomID] : null;
        }

        public List<RoomData> GetAllRooms()
        {
            return _rooms.Values.ToList();
        }

        public List<Vector2I> GetAllTilePositionsOfType(TileType searchedType)
        {
            var positions = new List<Vector2I>();

            foreach (var (position, type) in _mapData)
            {
                if (type == searchedType)
                {
                    positions.Add(position);
                }
            }

            return positions;
        }
    }

    public class RoomData
    {
        public int RoomID { get; set; }
        public List<Vector2I> Positions { get; set; } = new List<Vector2I>();
        public List<int> ConnectedRoomIDs { get; set; } = new List<int>();
    }
}
