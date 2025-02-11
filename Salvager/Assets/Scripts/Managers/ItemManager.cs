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
        // public ItemBehaviour GetItemPrefab(string itemDataIdentifier);
        public ItemBehaviour InstantiateItem(ItemData itemData, Transform parent = null);

        [Obsolete("Use InstantiateItem instead")]
        ItemBehaviour GetItemPrefab(string dataIdentifier);
    }

    public class ItemManager : MonoBehaviour, IItemManager
    {
        [Inject] private DiContainer _diContainer;

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

        public ItemBehaviour InstantiateItem(ItemData itemData, Transform parent = null)
        {
            var itemPrefab = GetItemPrefab(itemData.Identifier);

            var item = _diContainer.InstantiatePrefab(
                itemPrefab,
                parent?.position ?? Vector3.zero,
                Quaternion.identity,
                parent
            ).GetComponent<ItemBehaviour>();

            item.SetData(itemData);

            return item;
        }
    }
}