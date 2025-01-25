using UnityEngine;
using Zenject;

namespace Managers
{
    public interface ISpawnerManager
    {
        T Spawn<T>(T prefab, Vector2 position) where T : MonoBehaviour;
        T Spawn<T>(T prefab, Vector2 position, Transform parent) where T : MonoBehaviour;
        GameObject Spawn(GameObject prefab, Vector2 position);
    }

    public class SpawnerManager : MonoBehaviour, ISpawnerManager
    {
        [Inject] DiContainer _container;

        public Creature SpawnCreature(Creature creaturePrefab, Vector2 position)
        {
            var creature = _container.InstantiatePrefab(creaturePrefab, position, Quaternion.identity, null);
            return creature.GetComponent<Creature>();
        }

        public T Spawn<T>(T prefab, Vector2 position) where T : MonoBehaviour
        {
            return _container.InstantiatePrefab(prefab, position, Quaternion.identity, null).GetComponent<T>();
        }

        public T Spawn<T>(T prefab, Vector2 position, Transform parent) where T : MonoBehaviour
        {
            return _container.InstantiatePrefab(prefab, position, Quaternion.identity, parent).GetComponent<T>();
        }

        public GameObject Spawn(GameObject prefab, Vector2 position)
        {
            return _container.InstantiatePrefab(prefab, position, Quaternion.identity, null);
        }
    }
}