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
            from.RemoveItem(item);
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

    private ItemBehaviour AddItemToInventory(ItemBehaviour item)
    {
        if(item.Inventory != null)
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

        return item;
    }

    /// <summary>
    /// Remove an item from the inventory, THIS DOESNT DESTROY THE GAMEOBJECT
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItem(ItemBehaviour item)
    {
        _items.Remove(item);

        UnregisterItem(item);
        
        item.Inventory = null;

        Changed?.Invoke();
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
            return 1;

        return item.Count;
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
            item.Count = itemData.Count;
        }
    }

    public bool HasItem(string getIdentifier)
    {
        return GetItem(getIdentifier) is not null;
    }
}