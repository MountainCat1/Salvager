using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Managers;
using Services.MapGenerators;
using UnityEngine;
using Utilities;
using Zenject;

public interface IRoomDecorator
{
    void DecorateRooms(ICollection<RoomData> roomData, float tileSize);
}

public partial class RoomDecorator : MonoBehaviour, IRoomDecorator
{
    [Inject] private DiContainer _context = null!;

    [Inject] private IDataManager _dataManager = null!;

    [SerializeField] private GameObject roomMarker;

    [SerializeField] private RoomBlueprint startingRoomBlueprint;

    private List<Vector2> _occupiedPositions = new();

    public void DecorateRooms(ICollection<RoomData> roomData, float tileSize)
    {
        var roomQueue = new Queue<RoomData>(roomData);

        DecorateStartingRoom(roomQueue, tileSize);
        DecorateRoomsFromFeatures(roomQueue, tileSize);
        DecorateRemainingRoomsWithGenericBlueprints(roomQueue, tileSize);
    }

    private void DecorateStartingRoom(Queue<RoomData> roomQueue, float tileSize)
    {
        if (roomQueue.Count == 0)
        {
            Debug.LogError("No rooms to decorate.");
            return;
        }

        var startingRoom = roomQueue.Dequeue();
        DecorateRoom(startingRoom, startingRoomBlueprint, tileSize);
    }

    private void DecorateRoomsFromFeatures(Queue<RoomData> roomQueue, float tileSize)
    {
        var features = GameManager.GameSettings.Location.Features;

        foreach (var feature in features)
        {
            foreach (var blueprintName in feature.RoomBlueprints)
            {
                var roomBlueprint = LoadRoomBlueprint(blueprintName);
                if (roomBlueprint == null)
                    continue;

                DecorateRooms(roomQueue, roomBlueprint, tileSize);
            }
        }
    }

    private void DecorateRemainingRoomsWithGenericBlueprints(
        Queue<RoomData> roomQueue,
        float tileSize
    )
    {
        var genericRooms = GameManager.GameSettings.Location.Features
            .SelectMany(x => x.GenericRoomBlueprints)
            .ToList();

        if (!genericRooms.Any())
        {
            Debug.LogError("No generic room blueprints found.");
            return;
        }

        while (roomQueue.Any())
        {
            var blueprintName = genericRooms.RandomElement();
            var roomBlueprint = LoadRoomBlueprint(blueprintName);
            if (roomBlueprint == null)
                continue;

            DecorateRooms(roomQueue, roomBlueprint, tileSize);
        }
    }

    private void DecorateRooms(
        Queue<RoomData> roomQueue,
        RoomBlueprint roomBlueprint,
        float tileSize
    )
    {
        if (roomQueue.Count == 0)
        {
            Debug.LogError(
                "Not enough rooms to decorate all blueprints. Decorated rooms with this blueprint."
            );
        }

        DecorateRoom(roomQueue.Dequeue(), roomBlueprint, tileSize);
    }

    private RoomBlueprint LoadRoomBlueprint(string blueprintName)
    {
        var roomBlueprint = Resources.Load($"Rooms/{blueprintName}") as RoomBlueprint;

        if (roomBlueprint == null)
        {
            Debug.LogError($"Room blueprint {blueprintName} not found");
        }

        return roomBlueprint;
    }

    private void DecorateRoom(RoomData roomData, RoomBlueprint blueprint, float tileSize)
    {
        Debug.Log($"Decorating room {roomData.RoomID} with blueprint {blueprint.Name}");

        PlaceRoomMarker(roomData, blueprint, tileSize);
        SpawnProps(roomData, blueprint, tileSize);

        var enemies = new List<Creature>();

        foreach (var roomEnemy in blueprint.Enemies)
        {
            for (int i = 0; i < UnityEngine.Random.Range(roomEnemy.minAmount, roomEnemy.maxAmount); i++)
            {
                enemies.Add(roomEnemy.enemy);
            }
        }

        roomData.Enemies = enemies.ToArray();
        roomData.IsEntrance = blueprint.StartingRoom;
    }

    private void PlaceRoomMarker(
        RoomData roomData,
        RoomBlueprint blueprint,
        float tileSize
    )
    {
        var roomCenter = GetRoomCenter(roomData, tileSize);
        var marker = Instantiate(roomMarker, roomCenter, Quaternion.identity);
        marker.name = $"{blueprint.name} ({roomData.RoomID})";
    }

    private Vector2 GetRoomCenter(RoomData roomData, float tileSize)
    {
        var roomCenterX = (float)roomData.Positions.Average(i => i.x);
        var roomCenterY = (float)roomData.Positions.Average(i => i.y);
        return new Vector2(roomCenterX, roomCenterY) * tileSize;
    }

    private void SpawnProps(RoomData roomData, RoomBlueprint blueprint, float tileSize)
    {
        foreach (var prop in blueprint.Props)
        {
            for (int i = 0; i < prop.count; i++)
            {
                Vector2Int? randomPosition = null;

                switch (prop.position)
                {
                    case PropPosition.Anywhere:
                        randomPosition = GetRandomAvailablePosition(roomData);
                        break;
                    case PropPosition.Center:
                        randomPosition = GetCenterPosition(roomData);
                        break;
                    case PropPosition.Corner:
                        randomPosition = GetCornerPosition(roomData);
                        break;
                    case PropPosition.NotEdge:
                        randomPosition = GetNotEdgePosition(roomData);
                        break;
                    case PropPosition.Edge:
                        randomPosition = GetEdgePosition(roomData);
                        break;
                }

                if (randomPosition != null)
                {
                    var spawnPosition = (Vector2)randomPosition * tileSize +
                                        new Vector2(tileSize, tileSize) / 2 + prop.offset;
                    InstantiatePrefab(prop.prefab, spawnPosition);
                    _occupiedPositions.Add(randomPosition.Value);
                }
            }
        }
    }

    private Vector2Int? GetRandomAvailablePosition(RoomData roomData)
    {
        var availablePositions = roomData.Positions.Where(ValidatePosition).ToList();
        return availablePositions.Any() ? availablePositions.RandomElement() : null;
    }

    private Vector2Int? GetCenterPosition(RoomData roomData)
    {
        var minX = roomData.Positions.Min(p => p.x);
        var maxX = roomData.Positions.Max(p => p.x);
        var minY = roomData.Positions.Min(p => p.y);
        var maxY = roomData.Positions.Max(p => p.y);

        var centerPositions = roomData.Positions.Where(p =>
            p.x > minX && p.x < maxX && p.y > minY && p.y < maxY && ValidatePosition(p)
        ).ToList();

        return centerPositions.Any() ? centerPositions.RandomElement() : null;
    }


    private Vector2Int? GetCornerPosition(RoomData roomData)
    {
        var minX = roomData.Positions.Min(p => p.x);
        var maxX = roomData.Positions.Max(p => p.x);
        var minY = roomData.Positions.Min(p => p.y);
        var maxY = roomData.Positions.Max(p => p.y);

        var cornerPositions = new List<Vector2Int>
        {
            new(minX, minY),
            new(minX, maxY),
            new(maxX, minY),
            new(maxX, maxY)
        }.Where(ValidatePosition).ToList();

        return cornerPositions.Any() ? cornerPositions.RandomElement() : null;
    }

    private Vector2Int? GetEdgePosition(RoomData roomData)
    {
        var minX = roomData.Positions.Min(p => p.x);
        var maxX = roomData.Positions.Max(p => p.x);
        var minY = roomData.Positions.Min(p => p.y);
        var maxY = roomData.Positions.Max(p => p.y);

        var edgePositions = roomData.Positions.Where(p =>
            (p.x == minX || p.x == maxX || p.y == minY || p.y == maxY) && ValidatePosition(p)
        ).ToList();

        return edgePositions.Any() ? edgePositions.RandomElement() : null;
    }

    private Vector2Int? GetNotEdgePosition(RoomData roomData)
    {
        var minX = roomData.Positions.Min(p => p.x);
        var maxX = roomData.Positions.Max(p => p.x);
        var minY = roomData.Positions.Min(p => p.y);
        var maxY = roomData.Positions.Max(p => p.y);
        
        var notEdgePositions = roomData.Positions.Where(p =>
            (p.x != minX && p.x != maxX && p.y != minY && p.y != maxY) && ValidatePosition(p)
        ).ToList();
        
        return notEdgePositions.Any() ? notEdgePositions.RandomElement() : null;
    }

    private bool ValidatePosition(Vector2Int position)
    {
        return !_occupiedPositions.Contains(position);
    }

    private void InstantiatePrefab(GameObject prefab, Vector2 position)
    {
        Debug.Log($"Instantiating prefab {prefab.name} at {position}");
        _context.InstantiatePrefab(prefab, position, Quaternion.identity, null);
    }
}