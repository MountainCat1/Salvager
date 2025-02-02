using System;
using System.Collections.Generic;
using System.Linq;
using Items;
using UnityEngine;
using Zenject;

namespace Managers
{
    public interface IItemManager
    {
        public ItemPickup SpawnPickup(ItemBehaviour itemBehaviour, Vector3 position);
        public ItemBehaviour GetItemPrefab(string itemDataIdentifier);
    }

    public class ItemManager : MonoBehaviour, IItemManager
    {
        [Inject] private ISpawnerManager _spawnerManager;
        [Inject] private ICreatureEventProducer _creatureEventProducer;

        [SerializeField] private ItemPickup itemPickupPrefab;
        [SerializeField] private float forceOffset = 50f;

        private ICollection<ItemBehaviour> _items;

        private void Awake()
        {
            _items = Resources.LoadAll<ItemBehaviour>("Items");
            Debug.Log($"Loaded {_items.Count} items.\n{string.Join("\n", _items.Select(i => i.GetIdentifier()))}");
        }

        private void Start()
        {
            _creatureEventProducer.CreatureDied += OnCreatureDied;
        }

        private void OnCreatureDied(Creature creature, DeathContext deathContext)
        {
            var items = creature.Inventory.Items;

            foreach (var item in items.ToArray())
            {
                creature.Inventory.RemoveItem(item);
                SpawnPickup(item, creature.transform.position);
            }
        }

        public ItemPickup SpawnPickup(ItemBehaviour itemBehaviour, Vector3 position)
        {
            var itemPickup = _spawnerManager.Spawn(itemPickupPrefab, position, transform);

            itemPickup.SetItem(itemBehaviour);

            var force = new Vector2(
                UnityEngine.Random.Range(-forceOffset, forceOffset),
                UnityEngine.Random.Range(-forceOffset, forceOffset)
            );
            itemPickup.Rigidbody2D.AddForce(force);

            return itemPickup;
        }

        public ItemBehaviour GetItemPrefab(string itemDataIdentifier)
        {
            return _items.FirstOrDefault(i => i.GetIdentifier() == itemDataIdentifier);
        }
    }
}