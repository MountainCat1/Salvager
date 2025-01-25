using System;
using UnityEngine;
using Random = System.Random;

namespace Services.MapGenerators.GenerationSteps
{
    public class ConnectRoomsGenerationStep : GenerationStep
    {
        public override void Generate(GenerateMapData data, GenerateMapSettings settings, Random random)
        {
            var rooms = data.Rooms;
            
            void SetFloor(int x, int y)
            {
                data.SetTile(x, y, TileType.Floor);
            }
            
            for (int i = 0; i < rooms.Count - 1; i++)
            {
                var startRoom = rooms[i];
                var endRoom = rooms[i + 1];

                var start = startRoom.Positions[random.Next(startRoom.Positions.Count)];
                var end = endRoom.Positions[random.Next(endRoom.Positions.Count)];

                CreateCorridor(SetFloor, start, end);

                startRoom.ConnectedRoomIDs.Add(endRoom.RoomID);
                endRoom.ConnectedRoomIDs.Add(startRoom.RoomID);

                Debug.Log($"Corridor created between Room {startRoom.RoomID} and Room {endRoom.RoomID}");
            }
        }

        private void CreateCorridor(Action<int, int> setFloor, Vector2Int start, Vector2Int end)
        {
            var current = start;

            while (current != end)
            {
                if (current.x != end.x)
                {
                    current.x += Math.Sign(end.x - current.x);
                }
                else if (current.y != end.y)
                {
                    current.y += Math.Sign(end.y - current.y);
                }

                setFloor(current.x, current.y);
            }

            Debug.Log($"Corridor created from: {start} to {end}");
        }
    }
}