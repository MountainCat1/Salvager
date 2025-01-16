using UnityEngine;
using Zenject;

namespace Managers
{
    public interface ISpawnerManager
    {
        Creature SpawnCreature(Creature creaturePrefab, Vector2 position);
    }

    public class SpawnerManager : MonoBehaviour, ISpawnerManager
    {
        [Inject] DiContainer _container;

        public Creature SpawnCreature(Creature creaturePrefab, Vector2 position)
        {
            var creature = _container.InstantiatePrefab(creaturePrefab, position, Quaternion.identity, null);
            return creature.GetComponent<Creature>();
        }
    }
}