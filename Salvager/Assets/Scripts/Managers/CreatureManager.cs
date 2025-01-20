using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Managers
{
    public interface ICreatureManager
    {
        event Action<Creature> CreatureSpawned;
        ICollection<Creature> GetCreatures();
        public Creature SpawnCreature(Creature creaturePrefab, Vector3 position, Transform parent = null);
    }

    public class CreatureManager : MonoBehaviour, ICreatureManager
    {
        public event Action<Creature> CreatureSpawned;

        [Inject] private DiContainer _diContainer;
        [Inject] private ISpawnerManager _spawnerManager;

        private void Start()
        {
            var preSpawnedCreatures = FindObjectsOfType<Creature>();
            foreach (var creature in preSpawnedCreatures)
            {
                _diContainer.Inject(creature.gameObject);
                CreatureSpawned?.Invoke(creature);
            }
        }

        public Creature SpawnCreature(Creature creaturePrefab, Vector3 position, Transform parent = null)
        {
            var creatureGo = _spawnerManager.Spawn(
                prefab: creaturePrefab.gameObject,
                position: position
            );

            var creature = creatureGo.GetComponent<Creature>();

            CreatureSpawned?.Invoke(creature);
            
            return creature;
        }

        public ICollection<Creature> GetCreatures()
        {
            var creatures = FindObjectsOfType<Creature>(); // TODO: PERFORMANCE ISSUE
            return creatures;
        }
    }
}
