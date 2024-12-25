using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Managers
{
    public interface ICreatureManager
    {
        event Action<Creature> CreatureSpawned;
        ICollection<Creature> GetCreatures();
    }

    public class CreatureManager : MonoBehaviour, ICreatureManager
    {
        public event Action<Creature> CreatureSpawned;

        [Inject] private DiContainer _diContainer;

        private void Start()
        {
            var prespawnedCreatures = FindObjectsOfType<Creature>();
            foreach (var creature in prespawnedCreatures)
            {
                _diContainer.Inject(creature.gameObject);
                CreatureSpawned?.Invoke(creature);
            }
        }

        public void SpawnCreature(Creature creaturePrefab, Vector3 position, Transform parent = null)
        {
            var creatureGo = _diContainer.InstantiatePrefab(
                prefab: creaturePrefab.gameObject,
                position: position,
                rotation: Quaternion.identity,
                parentTransform: parent
            );

            var creature = creatureGo.GetComponent<Creature>();

            CreatureSpawned?.Invoke(creature);
        }

        public ICollection<Creature> GetCreatures()
        {
            var creatures = FindObjectsOfType<Creature>(); // TODO: PERFORMANCE ISSUE
            return creatures;
        }
    }
}
