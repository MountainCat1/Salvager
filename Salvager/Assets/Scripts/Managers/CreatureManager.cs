﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Managers
{
    public interface ICreatureManager
    {
        event Action<Creature> CreatureSpawned;
        bool IsAliveAndActive(Creature creature);
        ICollection<Creature> GetCreatures();
        IEnumerable<Creature> GetCreaturesAliveActive();
        IEnumerable<Creature> GetAliveCreatures();
        public Creature SpawnCreature(Creature creaturePrefab, Vector3 position, Transform parent = null);
        public Creature SpawnCreature(Creature creaturePrefab, Vector2Int position, Transform parent = null)
            => SpawnCreature(creaturePrefab, (Vector2)position, parent);
    }

    public class CreatureManager : MonoBehaviour, ICreatureManager
    {
        public event Action<Creature> CreatureSpawned;

        [Inject] private DiContainer _diContainer;
        [Inject] private ISpawnerManager _spawnerManager;

        private List<Creature> _creatures = new List<Creature>();

        private void Start()
        {
            var preSpawnedCreatures = FindObjectsOfType<Creature>();
            foreach (var creature in preSpawnedCreatures)
            {
                _diContainer.Inject(creature.gameObject);

                HandleNewCreature(creature);
            }
        }

        public IEnumerable<Creature> GetCreaturesAliveActive()
        {
            return _creatures.FindAll(x => x.Health.Alive && x.gameObject.activeInHierarchy);
        }

        public IEnumerable<Creature> GetAliveCreatures()
        {
            return _creatures.FindAll(x => x.Health.Alive && x.gameObject.activeInHierarchy);
        }

        public Creature SpawnCreature(Creature creaturePrefab, Vector3 position, Transform parent = null)
        {
            var creature = _spawnerManager.Spawn(
                prefab: creaturePrefab,
                position: position
            );

            HandleNewCreature(creature);

            return creature;
        }

        private void HandleNewCreature(Creature creature)
        {
            CreatureSpawned?.Invoke(creature);

            _creatures.Add(creature);

            creature.Health.Death += (DeathContext ctx) => { _creatures.Remove(creature); };
        }

        public bool IsAliveAndActive(Creature creature)
        {
            return creature.Health.Alive && creature.gameObject.activeInHierarchy;
        }

        public ICollection<Creature> GetCreatures()
        {
            return _creatures;
        }
    }
}