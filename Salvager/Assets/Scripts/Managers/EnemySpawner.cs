using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CreatureControllers;
using Data;
using Services.MapGenerators;
using UnityEngine;
using Utilities;
using Zenject;

namespace Managers
{
    public interface IEnemySpawner
    {
        void Initialize(MapData mapData, LocationData location);
    }

    public class EnemySpawner : MonoBehaviour, IEnemySpawner
    {
        [SerializeField] private float spawnRate = 0.05f;

        [SerializeField] private float minDistance = 20f;

        [SerializeField] private int minEnemiesPerRoom = 3; // TODO: Make this configurable

        [SerializeField] private int maxEnemiesPerRoom = 9; // TODO: Make this configurable

        [SerializeField] private int minRoomsWithEnemies = 3; // TODO: Make this configurable

        [SerializeField] private int maxRoomsWithEnemies = 3; // TODO: Make this configurable

        [SerializeField] private int minEnemiesPerSpawn = 1; // TODO: Make this configurable

        [SerializeField] private int maxEnemiesPerSpawn = 3; // TODO: Make this configurable

        [Inject] private ISpawnerManager spawnerManager;

        [Inject] private ICreatureManager creatureManager;

        [Inject] private IDataResolver dataResolver;

        private LocationData locationData;

        private MapData mapData;

        public void Initialize(MapData mapData, LocationData location)
        {
            locationData = location;
            this.mapData = mapData;

            SpawnRoomBasedEnemies(
                Random.Range(minRoomsWithEnemies, maxRoomsWithEnemies)
            );

            SpawnFeatureBasedEnemies(
                new Queue<LocationFeatureData>(
                    locationData.Features.Where(x => x.Enemies.Any())
                )
            );

            StartCoroutine(
                EnemySpawningCoroutine(
                    locationData.Features
                        .SelectMany(x => x.Enemies)
                        .Select(x => x.CreatureData)
                        .ToList()
                )
            );
        }

        private void SpawnRoomBasedEnemies(int roomsWithEnemies)
        {
            foreach (
                var room in mapData
                    .GetAllRooms()
                    .Where(room => room.Enemies?.Any() == true)
            )
            {
                room.Occupied = true;
                for (int i = 0; i < roomsWithEnemies; i++)
                {
                    var enemy = room.Enemies.RandomElement();
                    var position = GetRandomFloorPosition(room);
                    if (position != null)
                    {
                        creatureManager.SpawnCreature(enemy, position.Value);
                    }
                }
            }
        }

        private void SpawnFeatureBasedEnemies(Queue<LocationFeatureData> featuresQueue)
        {
            foreach (
                var room in mapData
                    .GetAllRooms()
                    .Where(x => !x.Occupied)
                    .OrderBy(_ => Random.value)
            )
            {
                if (featuresQueue.Count == 0)
                    break;

                LocationFeatureData feature = featuresQueue.Dequeue();
                room.Occupied = true;

                if (feature.Enemies == null || feature.Enemies.Count == 0)
                {
                    // If the feature has no enemies, just continue to the next room
                    continue;
                }

                int enemiesInRoom = Random.Range(minEnemiesPerRoom, maxEnemiesPerRoom);

                var enemy = feature.Enemies.RandomElement().CreatureData;
                var enemyPrefab = dataResolver.ResolveCreaturePrefab(enemy);

                for (int i = 0; i < enemiesInRoom; i++)
                {
                    var position = GetRandomFloorPosition(room);
                    if (position != null)
                    {
                        creatureManager.SpawnCreature(enemyPrefab, position.Value);
                    }
                }
            }
        }

        private Vector2Int? GetRandomFloorPosition(RoomData room)
        {
            return room.Positions
                .Where(pos => mapData.GetTileType(pos) == TileType.Floor)
                .RandomElement();
        }

        private IEnumerator EnemySpawningCoroutine(ICollection<CreatureData> enemies)
        {
            var rooms = mapData.GetAllRooms();
            var positions = rooms.SelectMany(x => x.Positions).ToList();

            while (true)
            {
                var enemy = enemies.RandomElement();

                // Look for position far away from player
                var playerCreatures = creatureManager
                    .GetCreaturesAliveActive()
                    .Where(x => x.Team == Teams.Player)
                    .ToArray();

                if (!playerCreatures.Any())
                {
                    Debug.Log("No player creatures found, stopping enemy spawning");
                    break;
                }

                Vector2Int? position = FindSuitableSpawnPosition(positions, playerCreatures);

                if (position != null)
                {
                    SpawnEnemiesAtPosition(enemy, position.Value, playerCreatures);
                }
                else
                {
                    Debug.LogWarning("Could not find a suitable position to spawn enemy");
                }

                yield return new WaitForSeconds(1f / spawnRate);
            }
        }

        private Vector2Int? FindSuitableSpawnPosition(
            List<Vector2Int> positions,
            Creature[] playerCreatures
        )
        {
            int attemptsLeft = 50;
            Vector2Int? position = null;

            while (attemptsLeft > 0)
            {
                position = positions.RandomElement();
                if (
                    playerCreatures.All(
                        x => Vector2.Distance(x.transform.position, position.Value) > minDistance
                    )
                )
                {
                    break;
                }

                attemptsLeft--;
            }

            return position;
        }

        private void SpawnEnemiesAtPosition(
            CreatureData enemy,
            Vector2Int position,
            Creature[] playerCreatures
        )
        {
            var enemiesToSpawn = Random.Range(minEnemiesPerSpawn, maxEnemiesPerSpawn);

            Debug.Log($"Spawning {enemiesToSpawn} {enemy.Name} at {position}");

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                var spawnedEnemy = SpawnEnemy(enemy, position);
                var controller = spawnedEnemy?.Controller as AiController;

                if (controller == null)
                {
                    Debug.LogError("Enemy spawned without AI controller");
                    break;
                }

                controller.Memorize(playerCreatures.RandomElement());
            }
        }

        private Creature SpawnEnemy(CreatureData enemy, Vector2Int position)
        {
            return creatureManager.SpawnCreature(
                dataResolver.ResolveCreaturePrefab(enemy),
                position
            );
        }
    }
}