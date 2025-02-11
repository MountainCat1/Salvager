using System.Collections.Generic;
using System.Linq;
using Data;
using Services.MapGenerators;
using UnityEngine;
using Utilities;
using Zenject;

namespace Managers
{
    public interface IEnemySpawner
    {
        public void Initialize(MapData mapData);
    }

    public class EnemySpawner : MonoBehaviour, IEnemySpawner
    {
        [SerializeField] private float spawnRate = 0.05f;
        private readonly int _minEnemiesPerRoom = 3; // TODO: Make this configurable
        private readonly int _maxEnemiesPerRoom = 9;// TODO: Make this configurable
        private readonly int _minRoomsWithEnemies = 3;// TODO: Make this configurable
        private readonly int _maxRoomsWithEnemies = 3;// TODO: Make this configurable

        [Inject] private ISpawnerManager _spawnerManager;
        [Inject] private ICreatureManager _creatureManager;
        [Inject] private IDataResolver _dataResolver;

        private MapData _mapData;

        public void Initialize(MapData mapData)
        {
            _mapData = mapData;
            
            var location = GameManager.GameSettings.Location;
            int roomsWithEnemiesCount = Random.Range(_minRoomsWithEnemies, _maxRoomsWithEnemies);

            SpawnRoomBasedEnemies(roomsWithEnemiesCount);
            
            var featuresWithEnemies = location.Features.Where(x => x.Enemies.Any());
            SpawnFeatureBasedEnemies(new Queue<LocationFeatureData>(featuresWithEnemies));
        }

        private void SpawnRoomBasedEnemies(int roomsWithEnemies)
        {
            foreach (var room in _mapData.GetAllRooms().Where(room => room.Enemies?.Any() == true))
            {
                room.Occupied = true;
                for (int i = 0; i < roomsWithEnemies; i++)
                {
                    var enemy = room.Enemies.RandomElement();
                    var position = GetRandomFloorPosition(room);
                    if (position != null)
                    {
                        _creatureManager.SpawnCreature(enemy, position.Value);
                    }
                }
            }
        }
        
        private void SpawnFeatureBasedEnemies(Queue<LocationFeatureData> featuresQueue)
        {
            foreach (var room in _mapData.GetAllRooms().Where(x => !x.Occupied).OrderBy(_ => Random.value))
            {
                if (featuresQueue.Count == 0) break;

                LocationFeatureData feature = featuresQueue.Dequeue();
                room.Occupied = true;

                if (feature.Enemies == null || feature.Enemies.Count == 0)
                {
                    // If the feature has no enemies, just continue to the next room
                    continue;
                }

                int enemiesInRoom = Random.Range(_minEnemiesPerRoom, _maxEnemiesPerRoom);

                var enemy = feature.Enemies.RandomElement().CreatureData;
                var enemyPrefab = _dataResolver.ResolveCreaturePrefab(enemy);

                for (int i = 0; i < enemiesInRoom; i++)
                {
                    var position = GetRandomFloorPosition(room);
                    if (position != null)
                    {
                        _creatureManager.SpawnCreature(enemyPrefab, position.Value);
                    }
                }
            }
        }

        private Vector2Int? GetRandomFloorPosition(RoomData room)
        {
            return room.Positions
                .Where(pos => _mapData.GetTileType(pos) == TileType.Floor)
                .RandomElement();
        }
    }
}