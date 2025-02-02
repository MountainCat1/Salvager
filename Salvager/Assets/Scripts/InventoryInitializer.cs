using System.Collections.Generic;
using System.Net.NetworkInformation;
using Items;
using UnityEngine;

namespace DefaultNamespace
{
    public class InventoryInitializer : MonoBehaviour
    {
        [SerializeField] private Creature creature;
        [SerializeField] private List<ItemBehaviour> items;
        
        void Start()
        {
            foreach (var itemPrefab in items)
            {
                var itemInInventory = creature.Inventory.AddItemFromPrefab(itemPrefab);
                itemInInventory.Use(new ItemUseContext()
                {
                    Creature = creature
                });
            }
        }
    }
}