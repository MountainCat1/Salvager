using System;
using System.Collections.Generic;
using Items;
using UnityEngine;
using Zenject;

public class Inventory
{
    public event Action Changed;
    public event Action<ItemBehaviour> ItemUsed;

    [Inject] private DiContainer _diContainer;
    
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
    
    public ItemBehaviour AddItem(ItemBehaviour itemPrefab)
    {
        if(itemPrefab == null)
            throw new NullReferenceException("Tried to add item to inventory that is null");
        
        var instantiateItem = _diContainer.InstantiatePrefab(
            itemPrefab,
            _transform.position,
            Quaternion.identity,
            _transform
        );
        
        var itemScript = instantiateItem.GetComponent<ItemBehaviour>();
        
        itemScript.Original = itemPrefab.Original == null 
            ? itemPrefab 
            : itemPrefab.Original;
        
        _items.Add(itemScript);
        
        RegisterItem(itemScript);
        
        Changed?.Invoke();

        return itemScript;
    }
    
    public void RemoveItem(ItemBehaviour item)
    {
        _items.Remove(item);
        
        UnregisterItem(item);
        
        Changed?.Invoke();
    }
    
    public ItemBehaviour GetItem(string identifier)
    {
        return _items.Find(x => x.GetIdentifier().Equals(identifier));
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
}