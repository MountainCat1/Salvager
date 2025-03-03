using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CreatureControllers;
using Data;
using Interactables;
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
        [SerializeField] private float enemyManaPerRoomAwake = 25f;

        [SerializeField] private float minEnemiesPerSpawn = 1;
        [SerializeField] private float maxEnemiesPerSpawn = 2;

        [SerializeField] private float minDistanceFromExitProp = 15f;

        [Inject] private ISpawnerManager spawnerManager;
        [Inject] private ICreatureManager creatureManager;
        [Inject] private IDataResolver dataResolver;

        private LocationData locationData;

        private MapData mapData;

        public void Initialize(MapData mapData, LocationData location)
        {
            locationData = location;
            this.mapData = mapData;

            SpawnRoomBasedEnemies();

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

        private void SpawnRoomBasedEnemies()
        {
            foreach (
                var room in mapData
                    .GetAllRooms()
                    .Where(room => room.Enemies?.Any() == true)
            )
            {
                if (room.Occupied)
                    continue;

                room.Occupied = true;
                foreach (var enemy in room.Enemies)
                {
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
                if(room.IsEntrance)
                    continue; // Do not spawn enemies in entrance room
                
                if (featuresQueue.Count == 0)
                    break;
                
                LocationFeatureData feature = featuresQueue.Dequeue();
                room.Occupied = true;

                if (feature.Enemies == null || feature.Enemies.Count == 0)
                {
                    // If the feature has no enemies, just continue to the next room
                    continue;
                }

                var manaUsed = 1f;

                while (manaUsed < enemyManaPerRoomAwake)
                {
                    var enemy = feature.Enemies.RandomElement().CreatureData;
                    var enemyPrefab = dataResolver.ResolveCreaturePrefab(enemy);
                    var position = GetRandomFloorPosition(room);

                    var entranceProp = FindObjectOfType<ExitObject>();
                    
                    // Do not spawn enemies near the exit prop or if somehow position was not found
                    if (position != null && Vector2.Distance(position.Value, entranceProp.transform.position) > minDistanceFromExitProp)
                    {
                        creatureManager.SpawnCreature(enemyPrefab, position.Value);
                    }

                    manaUsed += enemy.ManaCost < 1f ? 1f : enemy.ManaCost;
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

            float gatheredMana = 0f;

            while (true)
            {
                var enemy = enemies.RandomElement();

                while (enemy.ManaCost > gatheredMana)
                {
                    yield return new WaitForFixedUpdate();
                    gatheredMana += locationData.EnemySpawnManaPerSecond * Time.fixedDeltaTime;
                }

                gatheredMana -= enemy.ManaCost;

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
                    SpawnEnemiesAtPosition(enemy, position.Value);
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
                    playerCreatures
                    .All(x => Vector2.Distance(x.transform.position, position.Value) > minDistance)
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
            Vector2Int position
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

                var hostileCreaturesToSpawnedCreature = creatureManager
                    .GetCreaturesAliveActive()
                    .Where(x => x.Team == Teams.Player)
                    .ToArray();

                if (hostileCreaturesToSpawnedCreature.Length == 0)
                {
                    Debug.LogWarning("No player creatures found to memorize");
                    break;
                }

                var target = hostileCreaturesToSpawnedCreature.RandomElement();

                controller.Memorize(target);
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