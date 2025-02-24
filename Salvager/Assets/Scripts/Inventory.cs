using System;
using System.Collections.Generic;
using Items;
using Managers;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

public class Inventory
{
    public event Action Changed;
    public event Action<ItemBehaviour> ItemUsed;

    [Inject] private DiContainer _diContainer;
    [Inject] private IItemManager _itemManager;

    public IReadOnlyList<ItemBehaviour> Items => _items;

    private readonly List<ItemBehaviour> _items = new();

    private Transform _transform;

    public Inventory(Transform rootTransform)
    {
        _transform = rootTransform;

        foreach (Transform child in _transform)
        {
            var item = child.GetComponent<ItemBehaviour>();
            if (item != null)
            {
                _items.Add(item);
                RegisterItem(item);
            }
        }
    }

    public void TransferItem(Inventory from, ItemBehaviour item)
    {
        if (from == null)
            throw new NullReferenceException("Tried to transfer item from null inventory");

        if (item == null)
            throw new NullReferenceException("Tried to transfer null item");

        if (from == this)
            return;

        if (from._items.Contains(item))
        {
            from.DeleteItem(item);
            AddInstantiatedItem(item);
        }
    }

    public ItemBehaviour AddItemFromPrefab(ItemBehaviour itemPrefab)
    {
        if (itemPrefab == null)
            throw new NullReferenceException("Tried to add item to inventory that is null");

        // Instantiate the item first
        var itemInstance = InstantiateItemPrefab(itemPrefab);

        return AddItemToInventory(itemInstance);
    }

    public void AddInstantiatedItem(ItemBehaviour item)
    {
        if (item == null)
            throw new NullReferenceException("Tried to add a null item to inventory");

        item.transform.SetParent(_transform);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        AddItemToInventory(item);
    }

    /// <summary>
    /// Adds an item to the inventory based on the provided item data.
    /// For stackable items, increases the count of existing items.
    /// For non-stackable items, creates a new instance.
    /// </summary>
    /// <param name="itemData">The data of the item to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when itemData or itemData.Prefab is null.</exception>
    public void AddItem(ItemData itemData)
    {
        if (itemData == null)
            throw new ArgumentNullException(nameof(itemData));

        if (itemData.Prefab == null)
            throw new ArgumentNullException(nameof(itemData.Prefab));

        if (itemData.Prefab.Stackable == false)
        {
            var itemBehaviour = AddItemFromPrefab(itemData.Prefab);
            itemBehaviour.SetData(itemData);
            return;
        }

        var item = GetItem(itemData.Identifier);
        if (item is null)
        {
            var itemBehaviour = AddItemFromPrefab(itemData.Prefab);
            itemBehaviour.SetData(itemData);
        }
        else
        {
            item.Count += itemData.Count;
        }

        Changed?.Invoke();
    }

    private ItemBehaviour AddItemToInventory(ItemBehaviour item)
    {
        if (item.Inventory != null)
            throw new Exception("Item already belongs to an inventory");


        if (item.Stackable)
        {
            var existing = GetItem(item.GetIdentifier());
            if (existing is not null)
            {
                existing.Count += item.Count;
                Object.Destroy(item.gameObject); // Destroy redundant instance
                Changed?.Invoke();
                return existing;
            }
        }

        // Non-stackable or first-time stackable item
        item.Inventory = this;
        _items.Add(item);
        RegisterItem(item);

        Changed?.Invoke();
        return item;
    }

    private ItemBehaviour InstantiateItemPrefab(ItemBehaviour itemPrefab)
    {
        var item = _diContainer.InstantiatePrefab(
            itemPrefab,
            _transform.position,
            Quaternion.identity,
            _transform
        ).GetComponent<ItemBehaviour>();

        item.Original = itemPrefab.Original ?? itemPrefab;
        item.SetData(ItemData.FromPrefabItem(itemPrefab));

        return item;
    }

    /// <summary>
    /// Remove an item from the inventory, THIS DOESNT DESTROY THE GAMEOBJECT
    /// </summary>
    /// <param name="item"></param>
    public void DeleteItem(ItemBehaviour item)
    {
        _items.Remove(item);

        UnregisterItem(item);

        item.Inventory = null;

        Changed?.Invoke();
    }

    public void RemoveItems(string identifier, int count)
    {
        var item = GetItem(identifier);
        if (item == null)
            return;

        if (item.Stackable)
        {
            item.Count -= count;
            if (item.Count <= 0)
                DeleteItem(item);
        }
        else
        {
            for (var i = 0; i < count; i++)
            {
                var itemToRemove = GetItem(identifier);
                if (itemToRemove == null)
                    return;

                DeleteItem(itemToRemove);
            }
        }
    }

    public ItemBehaviour GetItem(string identifier)
    {
        return _items.Find(x => x.GetIdentifier().Equals(identifier));
    }

    public int GetItemCount(string identifier)
    {
        var item = GetItem(identifier);
        if (item == null)
            return 0;

        if (item.Stackable)
            return item.Count;

        return 1;
    }

    private void RegisterItem(ItemBehaviour item)
    {
        item.Used += HandleItemUsed;
    }

    private void UnregisterItem(ItemBehaviour item)
    {
        item.Used -= HandleItemUsed;
    }

    private void HandleItemUsed(ItemBehaviour item)
    {
        ItemUsed?.Invoke(item);
    }

    public void SetData(InventoryData dataInventory)
    {
        foreach (var itemData in dataInventory.Items)
        {
            var itemPrefab = _itemManager.InstantiateItem(itemData);
            var item = AddItemFromPrefab(itemPrefab);
            item.SetData(itemData);
        }
    }

    public bool HasItem(string getIdentifier)
    {
        return GetItem(getIdentifier) is not null;
    }
}