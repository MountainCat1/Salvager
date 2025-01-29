using System;
using UnityEngine;
using Zenject;

namespace Managers
{
    public interface ISpawnerManager
    {
        public event Action<Component> Spawned;

        T Spawn<T>(T prefab, Vector2 position) where T : Component;
        T Spawn<T>(T prefab, Vector2 position, Transform parent) where T : Component;
    }

    public class SpawnerManager : MonoBehaviour, ISpawnerManager
    {
        public event Action<Component> Spawned;

        [Inject] DiContainer _container;

        public Creature SpawnCreature(Creature creaturePrefab, Vector2 position)
        {
            var creature = _container
                .InstantiatePrefab(creaturePrefab, position, Quaternion.identity, null)
                .GetComponent<Creature>();
            
            Spawned?.Invoke(creature);

            return creature;
        }

        public T Spawn<T>(T prefab, Vector2 position) where T : Component
        {
            var spawnedObject = _container.InstantiatePrefab(prefab, position, Quaternion.identity, null).GetComponent<T>();

            Spawned?.Invoke(spawnedObject);
            
            return spawnedObject;
        }

        public T Spawn<T>(T prefab, Vector2 position, Transform parent) where T : Component
        {
            var spawnedObject =  _container.InstantiatePrefab(prefab, position, Quaternion.identity, parent).GetComponent<T>();

            Spawned?.Invoke(spawnedObject);
            
            return spawnedObject;
        }

        // public GameObject Spawn(GameObject prefab, Vector2 position)
        // {
        //     var spawnedObject = _container.InstantiatePrefab(prefab, position, Quaternion.identity, null);
        //     
        //     Spawned?.Invoke(spawnedObject);
        //     
        //     return spawnedObject;
        // }
    }
}