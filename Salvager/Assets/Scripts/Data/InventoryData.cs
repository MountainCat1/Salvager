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
    public bool ContainsItem(ItemData itemData)
    {
        return Items.Contains(itemData);
    }

    public static InventoryData FromInventory(Inventory inventory)
    {
        var data = new InventoryData { Items = new List<ItemData>() };
        foreach (var item in inventory.Items)
        {
            data.Items.Add(ItemData.FromItem(item));
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
    }
    
    public void TransferItem(ItemData itemData, InventoryData targetInventory)
    {
        if(ContainsItem(itemData) == false)
        {
            Debug.LogError("Tried to transfer item not found in inventory");
            return;
        }
        
        if(itemData.Stackable == false)
        {
            RemoveItem(itemData);
            targetInventory.AddItem(itemData);
            return;
        }
        
        if(itemData.Count == 1)
        {
            RemoveItem(itemData);
            targetInventory.AddItem(itemData);
            return;
        }
     
        itemData.Count--;
        var newItemData = DataCloner.Clone(itemData);
        newItemData.Count = 1;
        targetInventory.AddItem(newItemData);
    }

    public void RemoveItem(ItemData itemData)
    {
        Items.Remove(itemData);
    }
}