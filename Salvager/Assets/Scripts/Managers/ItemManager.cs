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
        public ItemBehaviour GetItemPrefab(string itemDataIdentifier);
    }

    public class ItemManager : MonoBehaviour, IItemManager
    {
        private ICollection<ItemBehaviour> _items;

        private void Awake()
        {
            _items = Resources.LoadAll<ItemBehaviour>("Items");
            Debug.Log($"Loaded {_items.Count} items.\n{string.Join("\n", _items.Select(i => i.GetIdentifier()))}");
        }

        public ItemBehaviour GetItemPrefab(string itemDataIdentifier)
        {
            return _items.FirstOrDefault(i => i.GetIdentifier() == itemDataIdentifier);
        }
    }
}