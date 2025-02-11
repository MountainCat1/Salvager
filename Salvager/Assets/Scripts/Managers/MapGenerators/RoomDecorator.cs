using System.Collections.Generic;
using System.Linq;
using Services.MapGenerators;
using UnityEngine;
using Utilities;
using Zenject;


public interface IRoomDecorator
{
    public void DecorateRooms(ICollection<RoomData> roomData, float tileSize);
}

public partial class RoomDecorator : MonoBehaviour, IRoomDecorator
{
    [Inject] private DiContainer _context = null!;
    [SerializeField] private List<RoomBlueprint> roomBlueprints = new();
    
    private List<Vector2> _occupiedPositions = new();

    public void DecorateRooms(ICollection<RoomData> roomData, float tileSize)
    {
        // TODO: Implement room decoration blueprint selection
        var roomQueue = new Queue<RoomData>(roomData);
        foreach (var blueprint in roomBlueprints)
        {
            for (int i = 0; i < blueprint.Count; i++)
            {
                DecorateRoom(roomQueue.Dequeue(), blueprint, tileSize);
            }
        }
    }

    private void DecorateRoom(RoomData roomData, RoomBlueprint blueprint, float tileSize)
    {
        Debug.Log($"Decorating room {roomData.RoomID} with blueprint {blueprint.Name}");
        
        // Spawn props
        foreach (var prop in blueprint.Props)
        {
            for (int i = 0; i < prop.count; i++)
            {
                var randomPosition = roomData.Positions
                    .Where(ValidatePosition)
                    .RandomElement();
                
                InstantiatePrefab(prop.prefab, (Vector2)randomPosition * tileSize + new Vector2(tileSize, tileSize) / 2);
                _occupiedPositions.Add(randomPosition);
            }
        }

        // Set Enemies
        roomData.Enemies = blueprint.Enemies;
        
        // Set Variables
        roomData.IsEntrance = blueprint.StartingRoom;
        
    }

    private bool ValidatePosition(Vector2Int position)
    {
        if(_occupiedPositions.Contains(position))
        {
            return false;
        }


        return true;
    }

    private void InstantiatePrefab(GameObject prefab, Vector2 position)
    {
        Debug.Log($"Instantiating prefab {prefab.name} at {position}");
        _context.InstantiatePrefab(prefab, position, Quaternion.identity, null);
    }
}