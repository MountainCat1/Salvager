using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Unity.VisualScripting;
using UnityEngine;

// ReSharper disable InconsistentNaming

[Serializable]
public class InventoryData
{
    public List<ItemData> Items = new();
    public event Action Changed;

    public bool ContainsItem(ItemData itemData)
    {
        return Items.Contains(itemData);
    }

    public static InventoryData FromInventory(Inventory inventory)
    {
        var data = new InventoryData { Items = new List<ItemData>() };
        foreach (var item in inventory.Items)
        {
            data.Items.Add(ItemData.FromItemPrefab(item));
        }

        return data;
    }

    public void AddItem(ItemData itemData)
    {
        if (itemData.Stackable == false)
        {
            Items.Add(itemData);
            return;
        }

        var item = Items.FirstOrDefault(x => x.Identifier == itemData.Identifier);
        if (item == null)
        {
            Items.Add(itemData);
        }
        else
        {
            item.Count += itemData.Count;
        }

        Changed?.Invoke();
    }

    public int TransferItem(ItemData itemData, InventoryData targetInventory, int amount = 1)
    {
        if (ContainsItem(itemData) == false)
        {
            Debug.LogError("Tried to transfer item not found in inventory");
            return 1;
        }

        if (itemData.Stackable == false)
        {
            RemoveItem(itemData);
            targetInventory.AddItem(itemData);
            Changed?.Invoke();
            return 1;
        }

        if (itemData.Count <= amount)
        {
            RemoveItem(itemData);
            targetInventory.AddItem(itemData);
            Changed?.Invoke();
            return itemData.Count;
        }

        itemData.Count -= amount;
        
        var newItemData = DataCloner.Clone(itemData);
        newItemData.Count = amount;
        targetInventory.AddItem(newItemData);
        Changed?.Invoke();

        return amount;
    }

    public void RemoveItem(ItemData itemData)
    {
        Items.Remove(itemData);
        Changed?.Invoke();
    }
}