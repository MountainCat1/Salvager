using System;
using System.Collections.Generic;
using System.Linq;
using Items;
using Pathfinding.Collections;

public enum ItemType
{
    Default,
    Weapon,
    Consumable,
    Armor,
    Quest,
    Junk
}

[Serializable]
public class ItemData
{
    public string Identifier;
    public int Count = 1;
    public bool Stackable;
    public ItemType Type;

    public static ItemData FromPrefabItem(ItemBehaviour item)
    {
        return new ItemData
        {
            Identifier = item.GetIdentifier(),
            Count = 1,
            Stackable = item.Stackable,
            Type = item is Weapon ? ItemType.Weapon : ItemType.Default,
            Prefab = item.IsOriginal ? item : throw new NotImplementedException($"Item {item} is not original")
        };
    }
    
    public static ItemData FromInstantiatedItem(ItemBehaviour item)
    {
        if(item.IsOriginal)
            throw new NotImplementedException($"Item {item} is original");

        return item.GetData();
    }
    
    public List<ItemWeaponModifier> Modifiers = new();
    
    [NonSerialized] public ItemBehaviour Prefab;
    
    public float GetApplied(WeaponPropertyModifiers property, float baseValue)
    {
        return baseValue + GetChange(property, baseValue);
    }

    public float GetChange(WeaponPropertyModifiers property, float baseValue)
    {
        var percentageModifier = Modifiers.OfType<WeaponValueModifier>()
            .Where(m => m.Type == property)
            .Sum(x => x.Value);

        if (percentageModifier <= -1)
        {
            return -baseValue;
        }
            
        return baseValue * percentageModifier;
    }
}

[Serializable]
public class ItemWeaponModifier
{
}

public enum WeaponPropertyModifiers
{
    Damage,
    AttackSpeed,
    Range,
}

[Serializable]
public class WeaponValueModifier : ItemWeaponModifier
{
    public WeaponPropertyModifiers Type;
    public float Value;
}

[Serializable]
public class WeaponSpecialModifier : ItemWeaponModifier
{
    public string Identifier { get; set; }
}